using System;
using Keiwando.BigInteger;

public interface IHasHpUI
{
    public event Action<BigInteger, BigInteger> OnUpdateMaxHPUI;
    public event Action<BigInteger> OnUpdateCurrenHPUI;
    public event Action OnResetHPUI;
    public event Action<bool> OnActiveHpUI;
}