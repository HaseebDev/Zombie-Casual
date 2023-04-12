/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "Assets/OSA/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using QuickType.StarReward;

// You should modify the namespace to your own or - if you're sure there won't ever be conflicts - remove it altogether
namespace CustomListView.StarReward
{
    // There are 2 important callbacks you need to implement, apart from Start(): CreateViewsHolder() and UpdateViewsHolder()
    // See explanations below
    public class StarRewardListAdapater : OSA<BaseParamsWithPrefab, StarRewardItemViewHolder>
    {
        // Helper that stores data and notifies the adapter when items count changes
        // Can be iterated and can also have its elements accessed by the [] operator
        public SimpleDataHelper<StarRewardItemModel> Data { get; private set; }


        #region OSA implementation

        protected override void Start()
        {
            Data = new SimpleDataHelper<StarRewardItemModel>(this);

            // Calling this initializes internal data and prepares the adapter to handle item count changes
            base.Start();

            // Retrieve the models from your data source and set the items count
            /*
            RetrieveDataAndUpdate(500);
            */
        }

        // This is called initially, as many times as needed to fill the viewport, 
        // and anytime the viewport's size grows, thus allowing more items to be displayed
        // Here you create the "ViewsHolder" instance whose views will be re-used
        // *For the method's full description check the base implementation
        protected override StarRewardItemViewHolder CreateViewsHolder(int itemIndex)
        {
            var instance = new StarRewardItemViewHolder();

            // Using this shortcut spares you from:
            // - instantiating the prefab yourself
            // - enabling the instance game object
            // - setting its index 
            // - calling its CollectViews()
            instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

            return instance;
        }

        // This is called anytime a previously invisible item become visible, or after it's created, 
        // or when anything that requires a refresh happens
        // Here you bind the data from the model to the item's views
        // *For the method's full description check the base implementation
        protected override void UpdateViewsHolder(StarRewardItemViewHolder newOrRecycled)
        {
            // In this callback, "newOrRecycled.ItemIndex" is guaranteed to always reflect the
            // index of item that should be represented by this views holder. You'll use this index
            // to retrieve the model from your data set

            StarRewardItemModel model = Data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateView(model);
        }


        // This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
        // especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
        // download request, if it's still in progress when the item goes out of the viewport.
        // <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
        // *For the method's full description check the base implementation
        /*
        protected override void OnBeforeRecycleOrDisableViewsHolder(MyListItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
        {
            base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }
        */

        // You only need to care about this if changing the item count by other means than ResetItems, 
        // case in which the existing items will not be re-created, but only their indices will change.
        // Even if you do this, you may still not need it if your item's views don't depend on the physical position 
        // in the content, but they depend exclusively to the data inside the model (this is the most common scenario).
        // In this particular case, we want the item's index to be displayed and also to not be stored inside the model,
        // so we update its title when its index changes. At this point, the Data list is already updated and 
        // shiftedViewsHolder.ItemIndex was correctly shifted so you can use it to retrieve the associated model
        // Also check the base implementation for complementary info
        /*
        protected override void OnItemIndexChangedDueInsertOrRemove(MyListItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
        {
            base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

            shiftedViewsHolder.titleText.text = Data[shiftedViewsHolder.ItemIndex].title + " #" + shiftedViewsHolder.ItemIndex;
        }
        */

        #endregion

        // These are common data manipulation methods
        // The list containing the models is managed by you. The adapter only manages the items' sizes and the count
        // The adapter needs to be notified of any change that occurs in the data list. Methods for each
        // case are provided: Refresh, ResetItems, InsertItems, RemoveItems

        #region data manipulation

        public void AddItemsAt(int index, IList<StarRewardItemModel> items)
        {
            // Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
            //YourList.InsertRange(index, items);
            //InsertItems(index, items.Length);

            Data.InsertItems(index, items);
        }

        public void RemoveItemsFrom(int index, int count)
        {
            // Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
            //YourList.RemoveRange(index, count);
            //RemoveItems(index, count);

            Data.RemoveItems(index, count);
        }

        public void SetItems(IList<StarRewardItemModel> items)
        {
            // Commented: the below 3 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
            //YourList.Clear();
            //YourList.AddRange(items);
            //ResetItems(YourList.Count);

            Data.ResetItems(items);
        }

        #endregion


        // Here, we're requesting <count> items from the data source
        void RetrieveDataAndUpdate(int count)
        {
            StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
        }

        // Retrieving <count> models from the data source and calling OnDataRetrieved after.
        // In a real case scenario, you'd query your server, your database or whatever is your data source and call OnDataRetrieved after
        IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
        {
            // Simulating data retrieving delay
            yield return new WaitForSeconds(.5f);

            var newItems = new StarRewardItemModel[count];

            // Retrieve your data here
            /*
            for (int i = 0; i < count; ++i)
            {
                var model = new MyListItemModel()
                {
                    title = "Random item ",
                    color = new Color(
                                UnityEngine.Random.Range(0f, 1f),
                                UnityEngine.Random.Range(0f, 1f),
                                UnityEngine.Random.Range(0f, 1f),
                                UnityEngine.Random.Range(0f, 1f)
                            )
                };
                newItems[i] = model;
            }
            */

            OnDataRetrieved(newItems);
        }

        void OnDataRetrieved(StarRewardItemModel[] newItems)
        {
            Data.InsertItemsAtEnd(newItems);
        }
    }

    // Class containing the data associated with an item
    public class StarRewardItemModel
    {
        public int mileStoneIndex = 0;
        public StarRewardDesignElement designElement;
        public Action<StarRewardDesignElement, RewardData> defaultReward;
        public Action<StarRewardDesignElement, RewardData> bpReward;
        public bool isLock = true;
        public bool isClaimed = false;
        public bool isClaimedDefault = false;

        public StarRewardItemModel(int mileStoneIndex, StarRewardDesignElement design,
            Action<StarRewardDesignElement, RewardData> defaultReward,
            Action<StarRewardDesignElement, RewardData> bpReward)
        {
            this.mileStoneIndex = mileStoneIndex;
            this.designElement = design;
            this.defaultReward = defaultReward;
            this.bpReward = bpReward;
        }

        public void SetLock(bool b)
        {
            isLock = b;
        }

        public void SetClaim(bool b)
        {
            isClaimed = true;
            isClaimedDefault = b;
        }
    }


    // This class keeps references to an item's views.
    // Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
    public class StarRewardItemViewHolder : BaseItemViewsHolder
    {
        /*
        public Text titleText;
        public Image backgroundImage;
        */

        public StarRewardUI starUi;


        // Retrieving the views from the item's root GameObject
        public override void CollectViews()
        {
            base.CollectViews();
            starUi = root.GetComponent<StarRewardUI>();

            // GetComponentAtPath is a handy extension method from frame8.Logic.Misc.Other.Extensions
            // which infers the variable's component from its type, so you won't need to specify it yourself
            /*
            root.GetComponentAtPath("TitleText", out titleText);
            root.GetComponentAtPath("BackgroundImage", out backgroundImage);
            */
        }

        public void UpdateView(StarRewardItemModel model)
        {
            if (starUi != null && model != null)
            {
                starUi.transform.localScale = Vector3.one;
                starUi.LoadMileStoneIndex(model.mileStoneIndex);
                starUi.Load(model.designElement);
                starUi.SetLock(model.isLock);

                starUi.ResetClaim();
                if (model.isClaimed)
                {
                    starUi.SetClaim(model.isClaimedDefault);
                }
                starUi.SetOnClickCallback(model.defaultReward, model.bpReward);
            }
        }

        // Override this if you have children layout groups or a ContentSizeFitter on root that you'll use. 
        // They need to be marked for rebuild when this callback is fired
        /*
        public override void MarkForRebuild()
        {
            base.MarkForRebuild();

            LayoutRebuilder.MarkLayoutForRebuild(yourChildLayout1);
            LayoutRebuilder.MarkLayoutForRebuild(yourChildLayout2);
            YourSizeFitterOnRoot.enabled = true;
        }
        */

        // Override this if you've also overridden MarkForRebuild() and you have enabled size fitters there (like a ContentSizeFitter)
        /*
        public override void UnmarkForRebuild()
        {
            YourSizeFitterOnRoot.enabled = false;

            base.UnmarkForRebuild();
        }
        */
    }
}