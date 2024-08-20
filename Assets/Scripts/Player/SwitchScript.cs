using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeaponsNS;

public class SwitchScript : MonoBehaviour
{
    public Slider timerSlider;
    public GameObject timerGO;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI magSizeText;
    public TextMeshProUGUI weaponInfoText;

    private Inventory inventory;
    private WeaponScript weaponScript;
    private WeaponInfoStruct weapon;
    private Coroutine switchingC;
    
    private bool isSwitching;
    private int selectedWeapon = 1;
    public int currentWeapon = 1;
    private const int InventorySize = 3;
    private Dictionary<int, WeaponInfoStruct> inventoryDict;
    
    
    private void Start()
    { 
        timerGO.SetActive(false);
        weaponScript = GetComponent<WeaponScript>();
        inventory = GetComponent<Inventory>();
        inventoryDict = inventory.InventoryDictReference; //referencia o dicionario do Inventario

        weapon = transform.GetChild(1).GetComponent<IWeaponDataProvider>().GetWeaponData();
        // Transform child = transform.GetChild(1); //tem um objeto a mais na cena em baixo do player
        // IWeaponDataProvider iWeaponDataProvider = child.GetComponent<IWeaponDataProvider>();
        // weapon = iWeaponDataProvider.GetWeaponData();
        
        weapon.fireTime = 60f / weapon.fireRate;
        inventoryDict.Add(1, weapon); //salva a arma primaria
        weaponScript.UpdateWeapon(weapon);

        weaponInfoText.text = weapon.caliber + " - " + weapon.weaponName;
        ammoText.text = weapon.ammo.ToString("D2") + "/"; //interage com o HUD
        totalAmmoText.text = weapon.totalAmmo.ToString("D3");
        magSizeText.text = weapon.magSize.ToString("D2");
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals)) LoadWeapon(); //{}{}
        
        if (isSwitching) return;
        if      (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventoryDict.Count >= 2) selectedWeapon = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventoryDict.Count >= 3) selectedWeapon = 3;
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            ++selectedWeapon;
            if (selectedWeapon > inventoryDict.Count) selectedWeapon = 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            --selectedWeapon;
            if (selectedWeapon < 1) selectedWeapon = inventoryDict.Count;
        }
        if (currentWeapon != selectedWeapon)
        {
            switchingC = StartCoroutine(SwitchWeapon());
        }
    }


    void LoadWeapon()
    {
        Transform child = transform.GetChild(inventoryDict.Count + 1); //pega a arma nova
        // var script = child.GetComponent<IWeaponDataProvider>();
        // WeaponInfoStruct data = script.GetWeaponData(); //pega as informacoes da arma
        WeaponInfoStruct data = child.GetComponent<IWeaponDataProvider>().GetWeaponData(); //pega as informacoes da arma
        data.fireTime = 60f / data.fireRate;

        if (inventoryDict.Count < InventorySize) //adiciona mais armas no inventario se tiver poucas
        {
            selectedWeapon = inventoryDict.Count + 1; //indice pra adicionar uma arma nova
            inventoryDict.Add(selectedWeapon, data);
            switchingC = StartCoroutine(SwitchWeapon());
        }
        //FAZER VERIFICACAO SE TA NA ARMA PRIMARIA E NAO DEIXAR COMPRAR {}{}
        else if (currentWeapon != 1)
        {
            Destroy(transform.GetChild(currentWeapon).gameObject);
            
            // // Instantiate the new weapon game object at the same position as the old one
            // GameObject newWeapon = Instantiate(newWeaponPrefab, oldWeaponPosition, Quaternion.identity); {}{}
            
            child.transform.SetSiblingIndex(currentWeapon);
            weapon = data;
            switchingC = StartCoroutine(SwitchWeapon());
        }
    }


    IEnumerator SwitchWeapon()
    {
        isSwitching = true;
        weaponScript.isSwitching = true;
        weaponScript.Stop();
        
        Transform currentWeaponT = transform.GetChild(currentWeapon);
        currentWeaponT.gameObject.SetActive(false); //esconde a arma atual
        inventoryDict[currentWeapon] = weapon; //salva a arma atual no inventario
        
        float switchTime = weapon.switchTime;
        timerGO.SetActive(true);
        timerSlider.maxValue = weapon.switchTime;
        while (switchTime >= 0)
        {
            timerSlider.value = switchTime;
            switchTime -= Time.deltaTime;
            yield return null;
        }
        
        Transform selectedWeaponT = transform.GetChild(selectedWeapon); //pega o transform novo
        selectedWeaponT.gameObject.SetActive(true); //e liga
        
        weapon = inventoryDict[selectedWeapon]; //carrega os valores da arma no ambiente de trabalho
        currentWeapon = selectedWeapon; //atualiza o indice da arma atual
        
        weaponInfoText.text = weapon.caliber + " - " + weapon.weaponName;
        ammoText.text = weapon.ammo.ToString("D2") + "/";
        totalAmmoText.text = weapon.totalAmmo.ToString("D3");
        magSizeText.text = weapon.magSize.ToString();
        timerGO.SetActive(false);
        
        weaponScript.UpdateWeapon(weapon);
        isSwitching = false;
        weaponScript.isSwitching = false;

    }

}
