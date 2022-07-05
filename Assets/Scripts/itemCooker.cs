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
    [Command(requiresAuthority = false)]
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
        //Foreach item/slot in furnace
        for (int i = 0; i < storage.storage.Count; i++)
        {
            //Do recipes
            foreach (inv_recipe rep in m_cookingRecipes)
            {
                //Check input id
                if (storage.storage[i].id == rep.inputItems[0])
                {
                    //Check input amount
                    if (storage.storage[i].amount >= rep.inputAmount[0])
                    {
                        //Remove input item
                        storage.storage[i].amount -= rep.inputAmount[0];
                        //Check if empty
                        if(storage.storage[i].amount <= 0) {
                            storage.storage.Remove(storage.storage[i]);
                        }
                        //Give output
                        inv_item it = inv_item.CreateInstance<inv_item>();
                        it.amount = rep.outputAmount;
                        it.id = rep.outputItem;
                        storage.GiveItem(it);
                    }
                }

            }
        }

        storage.CMD_UpdateStorage(storage.storage, storage.slots);
    }

    public void RPC_UpdateToggle(bool toggle)
    {
        m_flame.SetActive(toggle);
    }

}
