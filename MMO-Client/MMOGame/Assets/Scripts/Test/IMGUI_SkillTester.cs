using GameClient;
using HS.Protobuf.Combat.Skill;
using UnityEngine;

public class IMGUI_SkillTester : MonoBehaviour
{
    private string skillInput = "";

    void OnGUI()
    {
        // 输入框
        skillInput = GUI.TextField(new Rect(10, 10, 200, 30), skillInput);

        // 按钮
        if (GUI.Button(new Rect(220, 10, 100, 30), "测试技能"))
        {
            Debug.Log("测试技能指令: " + skillInput);
            TestSkill(skillInput);
        }
    }

    private void TestSkill(string skillCommand)
    {
        SpellCastRequest req = new SpellCastRequest() { Info = new CastInfo() };
        req.Info.SkillId = int.Parse(skillCommand);
        req.Info.CasterId = GameApp.entityId;
        NetManager.Instance.Send(req);
    }
}
