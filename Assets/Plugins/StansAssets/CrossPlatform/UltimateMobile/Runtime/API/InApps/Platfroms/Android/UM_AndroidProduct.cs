using System;
using SA.Android.Vending.BillingClient;

namespace SA.CrossPlatform.InApp
{
    [Serializable]
    public class UM_AndroidProduct : UM_AbstractProduct<AN_SkuDetails>, UM_iProduct
    {
        public override void OnOverride(AN_SkuDetails productTemplate) {
            m_id = productTemplate.Sku;
            m_price = productTemplate.Price;
            m_priceInMicros = productTemplate.PriceAmountMicros;
            m_priceCurrencyCode = productTemplate.PriceCurrencyCode;

            m_title = productTemplate.Title;
            m_description = productTemplate.Description;
        }
    }
}