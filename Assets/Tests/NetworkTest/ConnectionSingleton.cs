using System.Collections;
using System.Collections.Generic;
using System.Net;
using Tests.NetworkTest.Connections;
using UnityEngine;

public class ConnectionSingleton
{
    // Singleton
    private static ConnectionSingleton instance;

    public static ConnectionSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConnectionSingleton();
            }
            return instance;
        }
    }

    private ConnectionSingleton()
    {
    }
    
    // Informacoes de Player
    public string Player_Name;
    public IPAddress Player_IP;

    // Conection
    public Connection Connection;

    // Sincronizar objetos entre os jogadores
    List<SyncBehavior> sync_objects;
    // Lista de syncOjects
    // Tempo de atualização

    // Task de comunicação
    async Task SyncMsg()
    {
        while(sync_objects.Count != 0)
        {
            foreach(SyncBehavior obj in sync_objects){
                // Enviar por udp
            }

            // Esperar o tempo de atualização
        }
    }

    // Otmização Rede x Compressão
    /*
        Duas ideias

        1 = Ao passar objetos pela rede é possivel comprimir os dados ou não
        a ideia desse sistema é que ele se auto regule sobre quadno comprimir 
        objetos e quando não. Para isso, a ideia é que tenha um "ponto de divisão" 
        onde mensagens com tamanho, em bytes, for maior que esse ponto a mesagem
        seria comprimida. O Player teria 3 opções:
        - Priorizar Rede
        - Priorizar Hardware
        - Inteligente

        2 = Treinar um modelo para identificar se vale ou não apena 
        comprimir uma mensagem baseado em na sua TAG e Tamanho.
        O Player teria as seguintes opções
        - Desligado
        - Treinamento continuo
        - Treinamento inteligente
        - Treinamento On / Off (O player ativa ou desativa o treinamento do modelo)
        - Pré treinado

        Vantagens da ideia 1
        - Fácil de implementar
        - Mais leve

        Vantagens da ideia 2
        - Entrega um maior desempenho quando treinado
        - Enquanto treinando consume uma quantidade consideravel de resursos

    */

}
