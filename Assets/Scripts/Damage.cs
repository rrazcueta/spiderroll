[System.Serializable]
public class Damage{
    public int damage;
    public float stun;
    public DamageType damageType;
}

[System.Serializable]
public enum DamageType {Blunt, Cutting, Pierce, Fire, Magic, Lightning, Poison }