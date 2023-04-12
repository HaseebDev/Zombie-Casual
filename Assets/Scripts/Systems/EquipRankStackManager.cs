using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EquipRankStackManager
{
    public static bool CanGetEquipRank(int rank, int count)
    {
        return SaveManager.Instance.Data.ShopData.StackChestEquipRank[rank - 1] >= count;
    }

    public static int CheckAndReset(int rank, int randomRank, int stackNeed)
    {
        if (CanGetEquipRank(rank, stackNeed) || randomRank == rank)
        {
            ResetStackChestEquipRank(rank);
            return rank;
        }
        else
        {
            AddStackChestEquipRank(rank);
        }

        return randomRank;
    }

    public static void ResetStackChestEquipRank(int rank)
    {
        SaveManager.Instance.Data.ShopData.StackChestEquipRank[rank - 1] = 0;
    }

    public static void AddStackChestEquipRank(int rank)
    {
        SaveManager.Instance.Data.ShopData.StackChestEquipRank[rank - 1]++;
    }

    public static int GetStackEquipRank(int rank)
    {
        return SaveManager.Instance.Data.ShopData.StackChestEquipRank[rank - 1];
    }
}