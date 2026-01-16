using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using SA.Foundation.Templates;
using SA.Android.Vending.BillingClient;
using UnityEngine.Assertions;

namespace SA.CrossPlatform.InApp
{
    internal class UM_AndroidInAppClient : UM_AbstractInAppClient, 
        UM_iInAppClient, 
        AN_iBillingClientStateListener,
        AN_iPurchasesUpdatedListener,
        AN_iConsumeResponseListener
    {
        private AN_BillingClient m_BillingClient;
        private Action<SA_iResult> m_ConnectToServiceCallback;
        private readonly List<AN_Purchase> m_Purchases = new List<AN_Purchase>();
        private readonly List<AN_SkuDetails> m_Products = new List<AN_SkuDetails>();
       

        //--------------------------------------
        //  UM_AbstractInAppClient
        //--------------------------------------

        protected override void ConnectToService(Action<SA_iResult> callback)
        {
            m_ConnectToServiceCallback = callback;
            using (var builder = AN_BillingClient.NewBuilder())
            {
                builder.SetListener(this);
                builder.EnablePendingPurchases();
                builder.SetChildDirected(AN_BillingClient.ChildDirected.Unspecified);
                builder.SetUnderAgeOfConsent(AN_BillingClient.UnderAgeOfConsent.Unspecified);

                m_BillingClient = builder.Build();
                m_BillingClient.StartConnection(this);
            }
        }
        
        //--------------------------------------
        //  AN_iBillingClientStateListener
        //--------------------------------------
        
        public void OnBillingSetupFinished(SA_iResult billingResult)
        {
            if (billingResult.IsSucceeded)
            {
                //inapp
                var purchasesResult = m_BillingClient.QueryPurchases(AN_BillingClient.SkuType.inapp);
                if (purchasesResult.IsSucceeded)
                {
                    m_Purchases.AddRange(purchasesResult.Purchases);
                }
                else
                {
                    m_ConnectToServiceCallback.Invoke(purchasesResult);
                }
                
                //subs
                purchasesResult = m_BillingClient.QueryPurchases(AN_BillingClient.SkuType.subs);
                if (purchasesResult.IsSucceeded)
                {
                    m_Purchases.AddRange(purchasesResult.Purchases);
                }
                else
                {
                    m_ConnectToServiceCallback.Invoke(purchasesResult);
                }
                
                
                var skuDetailsLoader = new UM_AndroidSkuDetailsLoader();
                skuDetailsLoader.LoadSkuDetails(m_BillingClient, AN_BillingClient.SkuType.inapp, inapps =>
                {
                    m_Products.AddRange(inapps);
                    skuDetailsLoader.LoadSkuDetails(m_BillingClient, AN_BillingClient.SkuType.subs, subs =>
                    {
                        m_Products.AddRange(subs);
                        m_ConnectToServiceCallback.Invoke(billingResult);
                    });
                });
            }
            else
            {
                m_ConnectToServiceCallback.Invoke(billingResult);
            }
        }

        public void OnBillingServiceDisconnected()
        {
            
        }

        protected override Dictionary<string, UM_iProduct> GetServerProductsInfo() {
            var products = new Dictionary<string, UM_iProduct>();
            foreach (var product in m_Products) {
                var p = new UM_AndroidProduct();
                p.Override(product);

                products.Add(p.Id, p);
            }
            return products;
        }

        protected override void ObserveTransactions() {
            foreach (var purchase in m_Purchases) {
                var transaction = new UM_AndroidTransaction(purchase, isRestored: false);

                if (!UM_AndroidInAppTransactions.IsTransactionCompleted(transaction.Id)) {
                    UpdateTransaction(transaction);
                }
            }
        }

        //--------------------------------------
        //  UM_iInAppClient
        //--------------------------------------

        public void AddPayment(string productId)
        {
            var skuDetails = GetSkuDetails(productId);
            Assert.IsNotNull(skuDetails);
            
            var paramsBuilder = AN_BillingFlowParams.NewBuilder();
            paramsBuilder.SetSkuDetails(skuDetails);
            m_BillingClient.LaunchBillingFlow(paramsBuilder.Build());
        }
        
        //--------------------------------------
        //  AN_iPurchasesUpdatedListener
        //--------------------------------------
        
        public void onPurchasesUpdated(SA_iResult billingResult, List<AN_Purchase> purchases)
        {
            foreach (var purchase in purchases)
            {
                if (billingResult.IsSucceeded)
                {
                    m_Purchases.Add(purchase);
                }
                
                var transaction = new UM_AndroidTransaction(billingResult, purchase);
                UpdateTransaction(transaction);
            }

            if (purchases.Count == 0)
            {
                if (billingResult.IsFailed)
                {
                    var transaction = new UM_AndroidTransaction(billingResult, null);
                    UpdateTransaction(transaction);
                }
                else
                {
                    throw new InvalidEnumArgumentException("billingResult is Succeeded, but no products provided");
                }
            }
        }

        public void FinishTransaction(UM_iTransaction transaction) {

            if(transaction.State == UM_TransactionState.Failed) {
                //noting to finish since it's failed
                //it will not have product or transaction id
                return;
            }

            var skuDetails = GetSkuDetails(transaction.ProductId);
            if (skuDetails != null) {
                if (skuDetails.Type == AN_BillingClient.SkuType.inapp && skuDetails.IsConsumable) {
                    var purchase = (transaction as UM_AndroidTransaction).Purchase;
                    Assert.IsNotNull(purchase);
                    
                    var paramsBuilder = AN_ConsumeParams.NewBuilder();
                    paramsBuilder.SetPurchaseToken(purchase.PurchaseToken);
                        
                    m_BillingClient.ConsumeAsync(paramsBuilder.Build(), this);
                }
            } else {
                Debug.LogError("Transaction is finished, but no product found with such id");
            }
                

            UM_AndroidInAppTransactions.RegisterCompleteTransaction(transaction.Id);
        }
        
        //--------------------------------------
        //  AN_iConsumeResponseListener
        //--------------------------------------
        
        public void OnConsumeResponse(SA_iResult billingResult, string purchaseToken)
        {
            Assert.IsTrue(billingResult.IsSucceeded);
            AN_Purchase consumePurchase = null;
            foreach (var purchase in m_Purchases)
            {
                if (purchase.PurchaseToken.Equals(purchaseToken))
                {
                    consumePurchase = purchase;
                }
            }

            if (consumePurchase != null)
            {
                m_Purchases.Remove(consumePurchase);
            }
        }

        public void RestoreCompletedTransactions() 
        {
           foreach(var purchase in m_Purchases) 
           {
                var transaction = new UM_AndroidTransaction(purchase, isRestored: true);
                UpdateTransaction(transaction);
           }
        }

        private AN_SkuDetails GetSkuDetails(string sku)
        {
            foreach (var product in m_Products)
            {
                if (product.Sku.Equals(sku))
                {
                    return product;
                }
            }

            return null;
        }
       
    }
}