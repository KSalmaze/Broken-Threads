using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerBehavior : MonoBehaviour
{
    // Listeners
    // Quando a start rodar, adicionar a lista global de triggers
    // Quando o trigger for disparado, atualizar a lista de triggers disparados (host), e informar os outros jogadores

    protected abstract void OnTrigger();
}