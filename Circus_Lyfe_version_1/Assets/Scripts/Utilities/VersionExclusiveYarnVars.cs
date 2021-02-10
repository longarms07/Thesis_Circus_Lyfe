using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(InMemoryVariableStorage))]
public class VersionExclusiveYarnVars : MonoBehaviour
{
    [System.Serializable]
    public struct VarSetter
    {
        public string varName;
        public string webGLValue;
        public string androidValue;
    }

    public List<VarSetter> variables;


    // Start is called before the first frame update
    void Start()
    {
        InMemoryVariableStorage storage = GetComponent<InMemoryVariableStorage>();
        List<InMemoryVariableStorage.DefaultVariable> vars = new List<InMemoryVariableStorage.DefaultVariable>(storage.defaultVariables);
        foreach (VarSetter vS in variables)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                InMemoryVariableStorage.DefaultVariable newVar = new InMemoryVariableStorage.DefaultVariable();
                newVar.name = vS.varName;
                newVar.value = vS.webGLValue;
                newVar.type = Yarn.Value.Type.String;
                vars.Add(newVar);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                InMemoryVariableStorage.DefaultVariable newVar = new InMemoryVariableStorage.DefaultVariable();
                newVar.name = vS.varName;
                newVar.value = vS.androidValue;
                newVar.type = Yarn.Value.Type.String;
                vars.Add(newVar);
            }
        }
        storage.defaultVariables = vars.ToArray();
        storage.ResetToDefaults();
    }

    
}
