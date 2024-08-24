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
    // otimizacao do salmaze com varios computadores
    // testar ate quando vale comprimir uma mensagem → testar no load o ping e velocidade de processamento
    // testar de novo a cada [medida de tempo] se tem que atualizar essa medida de processamento/compressao
    //
    // fazer a granada/lanca granada analisar o movimento com um raycast, se bater em alguma coisa,
    //      transform=hit.position, se der pra mover, continua normalmente
    //
    // nao ta pegando as informacoes da arma nova corretamente depois que ta com o inventario cheio

    // a trabalho

    
    public Slider timerSlider;
    public GameObject timerGO;
    public Rigidbody bulletRB;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magSizeText;
    private WeaponInfoStruct weapon;
    
    private Coroutine reloadingC;
    private Coroutine shootingC;
    public bool isShootingShotgun;
    public bool isShootingAuto;
    public bool isReloading;

    private Dictionary<int, WeaponInfoStruct> inventoryDict;
    public Inventory inventory;
    public Camera mainCamera;

    private const int ShotgunDamage = 5;
    private const int ShotgunBulletCount = 10;
    private const int ShotgunSpread = 25;
    private const int AutoDamage = 35;
    private const int AutoSpread = 4;
    private bool hasAmmo = true;
    // VARIAVEL DE FIRETIME

    private void Start()
    {
        mainCamera = Camera.main;
        timerGO.SetActive(false);
        inventoryDict = inventory.InventoryDictReference; //referencia o dicionario do Inventario
        
        weapon = transform.GetChild(0).GetChild(1).GetComponent<IWeaponDataProvider>().GetWeaponData();
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
            if (isShootingAuto) StopCoroutine(shootingC);
            shootingC = StartCoroutine(ShootingShotgun());
        }
    }
    
    
    void Shoot(int damage, int bulletCount, int spread)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Rigidbody bullet = Instantiate(bulletRB, transform.position, transform.rotation);
            Vector3 forceDirection = mainCamera.transform.forward;
            float forceMagnitude = 25f + Random.Range(-spread/2, spread/2);
            bullet.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            
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
                if (hitObject.CompareTag("Player")) //{}{} VERIFICAR SE O LAYER [E DE PLAYER
                {
                    // float dot = Vector3.Dot(rayDirection, hit.normal); //dot nao ta funcionando por algum motivo
                    // comparar se o hit object foi diferente, pegar de uma variavel ou trocar se for diferente
                    bool dot = Random.Range(-1, 1) > 0;
                    hitObject.GetComponent<Health>().TakeDamage(damage, dot, hit.point);
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
            Shoot(AutoDamage, 1, AutoSpread);
            ammoText.text = weapon.ammo.ToString("D2") + "/";
            yield return new WaitForSeconds(weapon.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShootingAuto = false;
    }
    
    
    IEnumerator ShootingShotgun() //tiro alternativo / controlado
    {
        isShootingShotgun = true;
        Shoot(ShotgunDamage, ShotgunBulletCount, ShotgunSpread);
        yield return new WaitForSeconds(10*weapon.fireTime); //muda o tempo em relacao a fire rate da arma
        isShootingShotgun = false;
    }
    
    
    IEnumerator Reload(bool partial)
    {
        isReloading = true;
        if (isShootingAuto || isShootingShotgun)
        { StopCoroutine(shootingC); isShootingAuto = false; isShootingShotgun = false; }
        
        float reloadTime = partial ? weapon.reloadTimePartial : weapon.reloadTime;
        timerGO.SetActive(true);
        timerSlider.maxValue = weapon.reloadTime;
        while (reloadTime >= 0)
        {
            timerSlider.value = reloadTime;
            reloadTime -= Time.deltaTime;
            yield return null;
        }
        timerGO.SetActive(false);
        weapon.ammo = partial ? weapon.magSize + 1 : weapon.magSize;
        ammoText.text = weapon.ammo.ToString("D2") + "/";
        isReloading = false;
        hasAmmo = true;
    }
    
    
    public void UpdateWeapon(WeaponInfoStruct currentWeapon) //update the selected weapon
    {
        weapon = currentWeapon;
    }

    public WeaponInfoStruct GetWeapon()
    {
        return weapon;
    }
    
    
    public void Stop()
    {
        if (isReloading) StopCoroutine(reloadingC); isReloading = false;
        if (isShootingAuto || isShootingShotgun)
        { StopCoroutine(shootingC); isShootingAuto = false; isShootingShotgun = false; }
    }
}
