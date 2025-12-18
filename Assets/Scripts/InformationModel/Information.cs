public class Information
{
    public string id;
    public string content;
    public float age;            // 信息当前存在时间
    public float maxLifetime;    // 超过此值信息死亡
    public float decayRate;      // 每次tick老化速度
    public float baseSpreadChance;  // 基础传播概率
}
