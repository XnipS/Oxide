using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[RequireComponent(typeof(itemStorage))]
public class itemCooker : NetworkBehaviour
{
    public GameObject m_flame;
    public float m_cookSpeed;
    float currentCooldown;
    public int m_fuel;
    public int m_fuelAmount;
    public int m_waste;
    public inv_recipe[] m_cookingRecipes;
    [SyncVar]
    public bool cookingEnabled;

    itemStorage storage;

    void Start()
    {
        storage = GetComponent<itemStorage>();
    }

    public void CMD_AttemptToToggle()
    {
        //Turn on/off
        if (cookingEnabled)
        {
            cookingEnabled = false;
            RPC_UpdateToggle(false);
        }
        else
        {
            //Check for fuel
            if (storage.HasEnough(m_fuel, m_fuelAmount))
            {
                cookingEnabled = true;
                RPC_UpdateToggle(true);
            }
        }
    }

    void Update()
    {
        if (!isServer) { return; }
        if (cookingEnabled)
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
            else
            {
                currentCooldown = m_cookSpeed;
                //Check for fuel
                if (storage.HasEnough(m_fuel, m_fuelAmount))
                {
                    //Successful cook
                    DoCook();
                }
                else
                {
                    //No Fuel
                    cookingEnabled = false;
                    RPC_UpdateToggle(false);
                }
            }
        }
    }

    void DoCook()
    {
        //Use fuel
        storage.DestroyItem(m_fuel, m_fuelAmount);
        //Give waste
        if (m_waste != 0)
        {
            inv_item wa = inv_item.CreateInstance<inv_item>();
            wa.amount = m_fuelAmount;
            wa.id = m_waste;
            storage.GiveItem(wa);
        }
        //Do recipes
        foreach (inv_recipe rep in m_cookingRecipes)
        {
            //Check if can afford
            bool can = true;
            for (int x = 0; x < rep.inputItems.Length; x++)
            {
                //Check if afford
                if (!storage.HasEnough(rep.inputItems[x], rep.inputAmount[x]))
                {
                    can = false;
                }
            }
            if(!can) {break;}
            //Remove input
            for (int x = 0; x < rep.inputItems.Length; x++)
            {
                storage.DestroyItem(rep.inputItems[x], rep.inputAmount[x]);
            }
            //Give output
            inv_item it = inv_item.CreateInstance<inv_item>();
            it.amount = rep.outputAmount;
            it.id = rep.outputItem;
            storage.GiveItem(it);
        }
        storage.CMD_UpdateStorage(storage.storage, storage.slots);
    }

    public void RPC_UpdateToggle(bool toggle)
    {
        m_flame.SetActive(toggle);
    }

}
