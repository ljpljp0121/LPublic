
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg.Role
{
public sealed partial class BnCombo : Luban.BeanBase
{
    public BnCombo(JSONNode _buf) 
    {
        { if(!_buf["SkillID"].IsNumber) { throw new SerializationException(); }  SkillID = _buf["SkillID"]; }
        { if(!_buf["ComboID"].IsNumber) { throw new SerializationException(); }  ComboID = _buf["ComboID"]; }
        { var __json0 = _buf["NextSkill"]; if(!__json0.IsArray) { throw new SerializationException(); } NextSkill = new System.Collections.Generic.Dictionary<int, string>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int _k0;  { if(!__e0[0].IsNumber) { throw new SerializationException(); }  _k0 = __e0[0]; } string _v0;  { if(!__e0[1].IsString) { throw new SerializationException(); }  _v0 = __e0[1]; }  NextSkill.Add(_k0, _v0); }   }
        { if(!_buf["Group"].IsNumber) { throw new SerializationException(); }  Group = _buf["Group"]; }
        Group_Ref = null;
    }

    public static BnCombo DeserializeBnCombo(JSONNode _buf)
    {
        return new Role.BnCombo(_buf);
    }

    public readonly int SkillID;
    public readonly int ComboID;
    public readonly System.Collections.Generic.Dictionary<int, string> NextSkill;
    public readonly int Group;
    public Skill.SkillClip Group_Ref;
   
    public const int __ID__ = -1586058454;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef()
    {
        Group_Ref = TableSystem.GetVOData<Skill.TbSkillClip>().GetOrDefault(Group);
    }

    public override string ToString()
    {
        return "{ "
        + "SkillID:" + SkillID + ","
        + "ComboID:" + ComboID + ","
        + "NextSkill:" + Luban.StringUtil.CollectionToString(NextSkill) + ","
        + "Group:" + Group + ","
        + "}";
    }
}

}

