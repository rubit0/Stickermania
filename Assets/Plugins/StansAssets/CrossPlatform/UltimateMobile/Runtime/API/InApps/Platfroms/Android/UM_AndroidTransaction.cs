using System;
using SA.Foundation.Templates;
using SA.Android.Vending.BillingClient;

namespace SA.CrossPlatform.InApp
{
    [Serializable]
    public class UM_AndroidTransaction : UM_AbstractTransaction, UM_iTransaction
    {
        private AN_Purchase m_Purchase;

        public UM_AndroidTransaction(SA_iResult billingResult, AN_Purchase purchase) {

            if(billingResult.IsSucceeded) {
                SetPurchase(purchase, false);
            } else {
                m_state = UM_TransactionState.Failed;
                m_error = billingResult.Error;
            }
        }

        public UM_AndroidTransaction(AN_Purchase purchase, bool isRestored) {
            SetPurchase(purchase, isRestored);
        }

        private void SetPurchase(AN_Purchase purchase, bool isRestored) {
            m_Purchase = purchase;
            m_id = m_Purchase.OrderId;
            m_productId = m_Purchase.Sku;
            m_unitxTimestamp = m_Purchase.PurchaseTime;

            if(isRestored) {
                m_state = UM_TransactionState.Restored;
            } else {
                m_state = UM_TransactionState.Purchased;
            }
        }

        public AN_Purchase Purchase 
        {
            get { return m_Purchase; }
        }
    }
}