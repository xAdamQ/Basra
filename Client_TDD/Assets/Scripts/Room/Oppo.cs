using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOppo : IPlayerBase
{
}

public class Oppo : PlayerBase, IOppo
{
    public void Init(int indexInRoom)
    {
    }

    public void Throw(int cardId)
    {
        throw new NotImplementedException();
    }

    public void Distribute()
    {
        throw new NotImplementedException();
    }
}