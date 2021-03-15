

public class MonsterHTN
{
    public Task root;
    MonsterPlanner monsterPlanner;

    public MonsterHTN(MonsterPlanner _monsterPlanner)
    {
        monsterPlanner = _monsterPlanner;
        root = new BeMonsterTask(monsterPlanner);

    }
}