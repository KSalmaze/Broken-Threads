using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeaponsNS;
using Random = UnityEngine.Random;

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
    //  nao era pra dar pra recarregar quando totalAmmo==0
    
    public Slider timerSlider;
    public GameObject timerGO;
    public Rigidbody bulletRB;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI totalAmmoText;

    private WeaponInfoStruct weapon;
    private Coroutine reloadingC;
    private Coroutine shootingC;

    public bool isShooting;
    public bool isReloading;
    public bool isSwitching;

    public Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isSwitching || isReloading) return;
        if (weapon.ammo == 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.R)))
        {
            reloadingC = StartCoroutine(Reload(false)); //empty reload
        }
        if (Input.GetKeyDown(KeyCode.R) && weapon.ammo < weapon.magSize) //manual reload
        {
            reloadingC = StartCoroutine(Reload(weapon.isClosedBolt));
        }
        //checar duas vezes pra nao dar erro de recarregar e atirar ao mesmo tempo
        if (isSwitching || isReloading || isShooting) return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
            shootingC = StartCoroutine(weapon.isFullAuto ? ShootingM0() : ShootingM1());
        else if (Input.GetKeyDown(KeyCode.Mouse1)) shootingC = StartCoroutine(ShootingM1());
    }
    
    
    void Shoot()
    {
        for (int i = 0; i < weapon.bulletCount; i++)
        {
            Rigidbody bullet = Instantiate(bulletRB, transform.position, transform.rotation);
            Vector3 forceDirection = transform.up;
            float forceMagnitude = 25f + Random.Range(-weapon.spread/2, weapon.spread/2);
            bullet.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        
            int damage = weapon.damage;
            float rangeLeft = 4 * weapon.range;
            Vector3 rayOrigin = mainCamera.transform.position;
            Vector3 rayDirection = mainCamera.transform.forward;
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-weapon.spread/2, weapon.spread/2),
                Random.Range(-weapon.spread/2, weapon.spread/2), 0f);
            rayDirection = spreadRotation * rayDirection;

            while (damage > 0)
            {
                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rangeLeft))
                {
                    rangeLeft -= hit.distance;
                    damage -= Mathf.FloorToInt(weapon.damage * hit.distance /
                                               (4 * weapon.range)); //DMG * porcentagem de energia que a bala ainda tem

                    GameObject hitObject = hit.collider.gameObject;
                    // if (terreno) return {}{}
                    float dot = Vector3.Dot(rayDirection, hit.normal);
                    hitObject.GetComponent<Health>().TakeDamage(damage, dot);

                    //prepare to chain raycasts
                    rayOrigin = hit.point - hit.normal; // slightly offset to prevent self-collision
                    damage -= weapon.decay;
                }
                else damage = 0; //se nao acertou nada, para o while
            }
        }

    }
    
    
    IEnumerator ShootingM0() //tiro normal / full auto
    {
        isShooting = true;
        while (weapon.ammo > 0 && Input.GetKey(KeyCode.Mouse0)) //se tiver municao e continuar atirando
        {
            Shoot(); --weapon.ammo; //atira e desconta a municao
            ammoText.text = weapon.ammo.ToString("D2") + "/";
            yield return new WaitForSeconds(weapon.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShooting = false;
    }
    
    
    IEnumerator ShootingM1() //tiro alternativo / controlado
    {
        isShooting = true; int i = 0;
        while (weapon.ammo > 0 && i < weapon.burstSize)
        {
            ++i; Shoot(); --weapon.ammo;
            ammoText.text = weapon.ammo.ToString("D2") + "/";
            yield return new WaitForSeconds(weapon.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShooting = false;
    }
    
    
    IEnumerator Reload(bool partial)
    {
        isReloading = true;
        if (isShooting) StopCoroutine(shootingC); isShooting = false;
            
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

        weapon.totalAmmo += weapon.ammo;
        if (weapon.totalAmmo > weapon.magSize) //se tiver bastante municao
        {
            weapon.ammo = partial ? weapon.magSize + 1 : weapon.magSize;
            weapon.totalAmmo -= weapon.ammo;
        }
        else //se tiver pouca municao
        {
            weapon.ammo = weapon.totalAmmo;
            weapon.totalAmmo = 0;
        }
            
        ammoText.text = weapon.ammo.ToString("D2") + "/";
        totalAmmoText.text = weapon.totalAmmo.ToString("D3");
        isReloading = false;
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
        if (isShooting)  StopCoroutine(shootingC);  isShooting = false;
        if (isReloading) StopCoroutine(reloadingC); isReloading = false;
    }
}
