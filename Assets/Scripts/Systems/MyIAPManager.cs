//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Purchasing;
//using UnityEngine.Purchasing.Security;


//public interface IMyIAPManager
//{
//    List<string> ListProductConsumable { get; }
//    List<string> ListProductNonConsumable { get; }
//    List<string> ListProductSubscription { get; }

//    string GetProductPrice(string productID);
//    void PurchaseIAP(string productID, Action<bool> callback);
//    void RestorePurchase(string productID, Action<bool> callback);

//}

//// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
//// one of the existing Survival Shooter scripts.

//// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
//public class MyIAPManager : BaseSystem<MyIAPManager>, IStoreListener, IMyIAPManager
//{
//    public static MyIAPManager instance;

//    private List<string> _listProductConsumable;
//    private List<string> _listProductNonconsumable;
//    private List<string> _listProductSubscription;

//    public List<string> ListProductConsumable => _listProductConsumable;
//    public List<string> ListProductNonConsumable => _listProductNonconsumable;
//    public List<string> ListProductSubscription => _listProductSubscription;


//    private void Awake()
//    {
//        instance = this;
//    }

//    private static IStoreController m_StoreController;          // The Unity Purchasing system.
//    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

//    // Product identifiers for all products capable of being purchased: 
//    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
//    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
//    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

//    // General product identifiers for the consumable, non-consumable, and subscription products.
//    // Use these handles in the code to reference which product to purchase. Also use these values 
//    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
//    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
//    // specific mapping to Unity Purchasing's AddProduct, below.
//    public static string kProductIDConsumable = "consumable";

//    public override void Initialize(params object[] pars)
//    {
//        base.Initialize(pars);
//        // If we haven't set up the Unity Purchasing reference
//        if (m_StoreController == null)
//        {
//            // Begin to configure our connection to Purchasing
//            InitializePurchasing();
//        }
//    }

//    public void InitializePurchasing()
//    {
//        // If we have already connected to Purchasing ...
//        if (IsInitialized())
//        {
//            // ... we are done here.
//            return;
//        }

//        // Create a builder, first passing in a suite of Unity provided stores.
//        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

//        // Add a product to sell / restore by way of its identifier, associating the general identifier
//        // with its store-specific identifiers.
//        //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
//        // Continue adding the non-consumable product.

//        foreach (var item in ListProductConsumable)
//        {
//            builder.AddProduct(item, ProductType.Consumable);
//        }

//        foreach (var item in ListProductNonConsumable)
//        {
//            builder.AddProduct(item, ProductType.NonConsumable);
//        }

//        foreach (var item in ListProductSubscription)
//        {
//            builder.AddProduct(item, ProductType.Subscription);
//        }

//        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
//        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
//        UnityPurchasing.Initialize(this, builder);
//    }

//    void BuyProductID(string productId)
//    {
//        // If Purchasing has been initialized ...
//        if (IsInitialized())
//        {
//            // ... look up the Product reference with the general product identifier and the Purchasing 
//            // system's products collection.
//            Product product = m_StoreController.products.WithID(productId);

//            // If the look up found a product for this device's store and that product is ready to be sold ... 
//            if (product != null && product.availableToPurchase)
//            {
//                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
//                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
//                // asynchronously.
//                m_StoreController.InitiatePurchase(product);
//            }
//            // Otherwise ...
//            else
//            {
//                // ... report the product look-up failure situation  
//                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
//            }
//        }
//        // Otherwise ...
//        else
//        {
//            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
//            // retrying initiailization.
//            Debug.Log("BuyProductID FAIL. Not initialized.");
//        }
//    }

//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        // Purchasing has succeeded initializing. Collect our Purchasing references.
//        Debug.Log("OnInitialized: PASS");

//        // Overall Purchasing system, configured with products for this application.
//        m_StoreController = controller;
//        // Store specific subsystem, for accessing device-specific store features.
//        m_StoreExtensionProvider = extensions;


//        if (m_StoreController != null && m_StoreController.products != null)
//        {
//            foreach (var prod in m_StoreController.products.all)
//            {
//                Debug.Log($"[IAP] ProdID {prod.metadata.localizedTitle} - {prod.metadata.localizedPriceString}");
//            }
//        }

//    }

//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
//        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
//    }

//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//    {
//        OnPurchaseSuccess(args.purchasedProduct.definition.id);
//        return PurchaseProcessingResult.Complete;
//    }

//    //    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
//    //    {
//    //        bool validPurchase = true; // Presume valid for platforms with no R.V.

//    //        // Unity IAP's validation logic is only included on these platforms.
//    //#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
//    //        // Prepare the validator with the secrets we prepared in the Editor
//    //        // obfuscation window.
//    //        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
//    //            AppleTangle.Data(), Application.bundleIdentifier);

//    //        try
//    //        {
//    //            // On Google Play, result has a single product ID.
//    //            // On Apple stores, receipts contain multiple products.
//    //            var result = validator.Validate(e.purchasedProduct.receipt);
//    //            // For informational purposes, we list the receipt(s)
//    //            Debug.Log("Receipt is valid. Contents:");
//    //            foreach (IPurchaseReceipt productReceipt in result)
//    //            {
//    //                Debug.Log(productReceipt.productID);
//    //                Debug.Log(productReceipt.purchaseDate);
//    //                Debug.Log(productReceipt.transactionID);
//    //            }
//    //        }
//    //        catch (IAPSecurityException)
//    //        {
//    //            Debug.Log("Invalid receipt, not unlocking content");
//    //            validPurchase = false;
//    //        }
//    //#endif

//    //        if (validPurchase)
//    //        {
//    //            // Unlock the appropriate content here.
//    //            OnPurchaseSuccess(e.purchasedProduct.definition.id);
//    //        }

//    //        return PurchaseProcessingResult.Complete;
//    //    }

//    private void OnPurchaseSuccess(string id)
//    {

//        _currentBuyingTask?.Invoke(true);
//    }

//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
//        _currentBuyingTask?.Invoke(false);
//    }

//    #region My Methods

//    public bool IsInitialized()
//    {
//        // Only say we are initialized if both the Purchasing references are set.
//        return m_StoreController != null && m_StoreExtensionProvider != null;
//    }

//    private Action<bool> _currentBuyingTask = null;


//    public string GetProductPrice(string productID)
//    {
//        if (m_StoreController != null && m_StoreController.products != null)
//            return m_StoreController.products.WithID(productID).metadata.localizedPriceString;
//        else
//            return "";
//    }

//    public void PurchaseIAP(string productID, Action<bool> callback)
//    {
//        _currentBuyingTask = callback;
//        BuyProductID(productID);
//    }

//    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
//    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
//    public void RestorePurchase(string productID, Action<bool> callback)
//    {
//        // If Purchasing has not yet been set up ...
//        if (!IsInitialized())
//        {
//            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
//            Debug.Log("RestorePurchases FAIL. Not initialized.");
//            return;
//        }

//        // If we are running on an Apple device ... 
//        if (Application.platform == RuntimePlatform.IPhonePlayer ||
//            Application.platform == RuntimePlatform.OSXPlayer)
//        {
//            // ... begin restoring purchases
//            Debug.Log("RestorePurchases started ...");

//            // Fetch the Apple store-specific subsystem.
//            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
//            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
//            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
//            apple.RestoreTransactions((result) =>
//            {
//                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
//                // no purchases are available to be restored.
//                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
//            });
//        }
//        // Otherwise ...
//        else
//        {
//            // We are not running on an Apple device. No work is necessary to restore purchases.
//            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
//        }
//    }

//    #endregion
//}

