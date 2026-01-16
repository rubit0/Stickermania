using System;
using System.Collections.Generic;
using SA.Android;
using SA.Android.Vending.Billing;
using SA.Android.Vending.BillingClient;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.CrossPlatform.InApp
{
    public class UM_AndroidSkuDetailsLoader : AN_iSkuDetailsResponseListener
    {
        private event Action<List<AN_SkuDetails>> m_Callback;

        public void LoadSkuDetails(AN_BillingClient client, AN_BillingClient.SkuType skuType, Action<List<AN_SkuDetails>> callback)
        {
            m_Callback = callback;
            var paramsBuilder = AN_SkuDetailsParams.NewBuilder();
            paramsBuilder.SetType(skuType);
            
            var skusList = new List<string>();
            foreach (var product in AN_Settings.Instance.InAppProducts)
            {
                if (product.Type == skuType)
                {
                    skusList.Add(product.Sku);
                }
            }
            
            paramsBuilder.SetSkusList(skusList);
            client.QuerySkuDetailsAsync(paramsBuilder.Build(), this);
        }

        public void OnSkuDetailsResponse(SA_Result billingResult, List<AN_SkuDetails> skuDetailsList)
        {

            if (billingResult.IsSucceeded)
            {
                var result = new List<AN_SkuDetails>();
                foreach (var product in skuDetailsList)
                {
                    var settingsProduct = GetProductFromSettings(product.Sku);
                    JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(product), settingsProduct);
                    result.Add(settingsProduct);
                }
                m_Callback.Invoke(result);
            }
            else
            {
                m_Callback.Invoke(new List<AN_SkuDetails>());
            }
        }

        private AN_SkuDetails GetProductFromSettings(string sku)
        {
           foreach (var product in AN_Settings.Instance.InAppProducts)
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