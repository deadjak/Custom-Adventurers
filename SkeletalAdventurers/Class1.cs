using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using AbraxisToolset;
using Necro;
using HBS;
using HBS.Collections;
using HBS.Data;
using HBS.Logging;
using HBS.Math;
using HBS.Text;
using Partiality.Modloader;
using MonoMod;
using MonoMod.ModInterop;
using Necro.Data.Stats;

namespace CustomAdventurers
{

    [MonoMod.MonoModPatch("global::Necro.CharacterSelectorCamera")]
    class patch_CharacterSelectorCamera
    {
        [MonoMod.MonoModIgnore]
        private string[] actorIds;
        [MonoMod.MonoModIgnore]
        private void AddActorIfValid(List<string> actorList, string id) { }
        string[] NewAIDs;

        public void InitSelectableCharacterIds()
        {   List<string> list = new List<string>();
            this.AddActorIfValid(list, "BlackguardMale");
            this.AddActorIfValid(list, "BlackguardFemale");
            if (AppPackageUtil.IsOwner("necro.dlc.01"))
            {
                this.AddActorIfValid(list, "BruteMale");
                this.AddActorIfValid(list, "BruteFemale");
            }
            if (AppPackageUtil.IsOwner("necro.dlc.02"))
            {
                this.AddActorIfValid(list, "ArcanistMale");
                this.AddActorIfValid(list, "ArcanistFemale");
            }
            this.GetCharacterId();
            foreach (string line in NewAIDs)
            {
                this.AddActorIfValid(list, line);
            }
            this.actorIds = list.ToArray();

        }

        public void GetCharacterId()
        {
            DataManager man = new DataManager();
            this.NewAIDs = man.GetSelectableCharacterIds();
        }
    }
    [MonoMod.MonoModPatch("global::Necro.ItemDef")]
    class patch_ItemDef
    {
        [MonoMod.MonoModIgnore]
        private string actorBodyRef;
        public string GetFullActorBodyRef(ActorBody.Gender gender)
        {
            if (string.IsNullOrEmpty(this.actorBodyRef))
            {
                return string.Empty;
            }
            if (gender == ActorBody.Gender.Male)
            {
                try
                {
                    if (ThirdPersonCameraControl.HasInstance && ThirdPersonCameraControl.Instance.CharacterActor.actorDefId.Contains("PC-"))
                    {
                        return this.actorBodyRef;
                    }
                }
                catch
                {
                    return this.actorBodyRef + "_male";
                }
                return this.actorBodyRef + "_male";
            }
            if (gender == ActorBody.Gender.Female)
            {
                return this.actorBodyRef + "_female";
            }
            return this.actorBodyRef;
        }
    }
    [MonoMod.MonoModPatch("global::Necro.DataManager")]
    class patch_DataManager
    {
        public string[] GetSelectableCharacterIds()
        {          
            List<string> list = new List<string>(LazySingletonBehavior<DataManager>.Instance.ActorDefs.Count);
            foreach (KeyValuePair<string, ActorDef> keyValuePair in LazySingletonBehavior<DataManager>.Instance.ActorDefs)
            {
                if (DataManager.IsSelectableActor(keyValuePair.Value))
                {
                    list.Add(keyValuePair.Key);
                }
            }
            return list.ToArray();
        }
        public static bool IsSelectableActor(ActorDef def)
        {
            return def.tags.Contains("pc");
        }
    }
}

