using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WeaponsNS;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public class WeaponScript : MonoBehaviour
{
    // TO-DO
    //  > colocar barrinha de reload embaixo da crosshair, branco com fundo preto e chanfro vermelho do parcial
    //  
    //  investigar se tem que puxar o bolt pra tras quando acaba a bala de uma closed bolt
    // 
    //  procurar saber como que faz pra ver o tempo que cada script demora pra executar
    //      se esse script for muito pesado, testar otimizar trocando variaveis da página weapon
    //      pra variaveis locais
    //
    //  fazer compras de arma terem um cooldown pra nao travar o script de trocar de arma
    //
    // fazer floating number damage indicators !!
    //
    // fazer formula pra aumentar o spread da arma com fogo continuo - log
    //
    // fazer slow ao tomar dano, num ienummerator com while pra aumentar a speed por tempo, assim como o slider aqui
    //
    //  verificar comentarios com {}{}
    //
    // otimizacao do Salmaze com varios computadores
    // testar ate quando vale comprimir uma mensagem → testar no load o ping e velocidade de processamento
    // testar de novo a cada [medida de tempo] se tem que atualizar essa medida de processamento/compressao
    //
    // fazer a granada/lanca granada analisar o movimento com um raycast, se bater em alguma coisa,
    //      transform=hit.position, se der pra mover, continua normalmente
    //
    // nao ta pegando as informacoes da arma nova corretamente depois que ta com o inventario cheio

    // a trabalho
    //
    
    [Header("UI Elements")]
    public Slider timerSlider;
    public GameObject timerGO;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magSizeText;
    private WeaponInfoStruct weapon;
    
    [Header("Coroutines")]
    private Coroutine reloadingC;
    private Coroutine shootingC;
    public bool isShootingShotgun;
    public bool isShootingAuto;
    public bool isReloading;
    private bool hasAmmo = true;
    
    [Header("Definitions")]
    public Camera mainCamera;
    private Transform muzzle;
    public AudioSource audioSource;
    public AudioClip autoShootSound;
    public AudioClip shotgunShootSound;
    public GameObject muzzleFlash;
    
    public Inventory inventory;
    private Dictionary<int, WeaponInfoStruct> inventoryDict;
    
    //Shotgun
    private const int ShotgunDamage = 10;
    private const int ShotgunBulletCount = 10;
    private const int ShotgunSpread = 15;
    
    //Auto
    private const int AutoDamage = 35;
    private const int AutoBulletCount = 1;
    private const int AutoSpread = 4;
    

    private void Start()
    {
        mainCamera = Camera.main;
        timerGO.SetActive(false);
        inventoryDict = inventory.InventoryDictReference; //referencia o dicionario do Inventario
        
        //load weapon info
        weapon = transform.GetChild(0).GetChild(1).GetComponent<IWeaponDataProvider>().GetWeaponData();
        muzzle = transform.GetChild(0).GetChild(1).GetChild(0);
        // Transform child = transform.GetChild(0);
        // IWeaponDataProvider iWeaponDataProvider = child.GetComponent<IWeaponDataProvider>();
        // weapon = iWeaponDataProvider.GetWeaponData();
        
        weapon.fireTime = 60f / weapon.fireRate;
        inventoryDict.Add(1, weapon); //salva a arma primaria

        ammoText.text = weapon.ammo.ToString("D2") + "/"; //interage com o HUD
        magSizeText.text = weapon.magSize.ToString("D2");
    }
    
    private void Update()
    {
        if (isReloading) return;
        if (weapon.ammo == 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.R)))
        {
            reloadingC = StartCoroutine(Reload(false)); //empty reload
        }
        if (Input.GetKeyDown(KeyCode.R) && weapon.ammo < weapon.magSize) //manual reload
        {
            reloadingC = StartCoroutine(Reload(weapon.isClosedBolt));
        }
        //checar duas vezes pra nao dar erro de recarregar e atirar ao mesmo tempo
        if (isReloading || isShootingShotgun) return;
        if (Input.GetKeyDown(KeyCode.Mouse0)) shootingC = StartCoroutine(ShootingAuto());
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (isShootingAuto) { StopCoroutine(shootingC); isShootingAuto = false; }
            shootingC = StartCoroutine(ShootingShotgun());
        }
    }
    
    
    private void Shoot(int damage, int bulletCount, int spread)
    {
        audioSource.PlayOneShot(autoShootSound);
        GameObject muzzleFlareInstantiate = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
        Destroy(muzzleFlareInstantiate, Time.deltaTime);
        
        for (int i = 0; i < bulletCount; i++)
        {
            float rangeLeft = 4 * weapon.range;
            Vector3 rayOrigin = mainCamera.transform.position;
            Vector3 rayDirection = mainCamera.transform.forward;
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spread/2, spread/2),
                                                         Random.Range(-spread/2, spread/2), 0f);
            rayDirection = spreadRotation * rayDirection;
            
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rangeLeft))
            {
                damage -= Mathf.FloorToInt(damage * hit.distance / (4 * weapon.range));
                                            //DMG * porcentagem de energia que a bala ainda tem
                
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Player"))
                {
                    // comparar se o hit object foi diferente, pegar de uma variavel ou trocar se for diferente
                    hitObject.GetComponent<Health>().TakeDamage(damage, hit.point, transform);
                }
            }
            
            --weapon.ammo;
            ammoText.text = weapon.ammo.ToString("D2") + "/";
            if (weapon.ammo == 0) { hasAmmo = false; break; }
        }
    }
    
    
    IEnumerator ShootingAuto() //tiro normal / full auto
    {
        isShootingAuto = true;
        while (hasAmmo && Input.GetKey(KeyCode.Mouse0)) //se tiver municao e continuar atirando
        {
            Shoot(AutoDamage, AutoBulletCount, AutoSpread);
            yield return new WaitForSeconds(weapon.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShootingAuto = false;
    }
    
    
    IEnumerator ShootingShotgun() //tiro alternativo / controlado
    {
        isShootingShotgun = true;
        Shoot(ShotgunDamage, ShotgunBulletCount, ShotgunSpread);
        yield return new WaitForSeconds(10 * weapon.fireTime);
        isShootingShotgun = false;
    }
    
    
    IEnumerator Reload(bool partial)
    {
        isReloading = true;
        if (isShootingAuto || isShootingShotgun)
        { StopCoroutine(shootingC); isShootingAuto = false; isShootingShotgun = false; }
        
        // float reloadTime = partial ? weapon.reloadTimePartial : weapon.reloadTime;
        float reloadTime = weapon.reloadTime;

        timerGO.SetActive(true);
        timerSlider.maxValue = weapon.reloadTime;
        while (reloadTime >= 0)
        {
            timerSlider.value = reloadTime;
            reloadTime -= Time.deltaTime;
            yield return null;
        }
        timerGO.SetActive(false);
        // weapon.ammo = partial ? weapon.magSize + 1 : weapon.magSize;
        weapon.ammo = weapon.magSize;
        ammoText.text = weapon.ammo.ToString("D2") + "/";
        isReloading = false;
        hasAmmo = true;
    }
    
    
    public void Stop()
    {
        if (isReloading) { StopCoroutine(reloadingC); isReloading = false; }
        if (isShootingAuto || isShootingShotgun)
        { StopCoroutine(shootingC); isShootingAuto = false; isShootingShotgun = false; }
    }
}
