using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

public class CharacterCreator : ScriptableWizard
{
    public enum CHR
    {
        Player, Enemy
    }

    public CHR Type = CHR.Player;

    public GameObject CharacterMesh; //To Create
    public GameObject CharacterReference; //to use as reference
    public UnityEditor.Animations.AnimatorController AnimatorController; //to use as reference
    public string RightHandBone = "Bip001 R Hand"; //to use as reference
    public string LeftHandBone = "Bip001 L Hand"; //to use as reference

    [MenuItem("PolygonR/Create Character Wizard...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CharacterCreator>("PolygonR : Create Character", "Create Player");
    }

    void OnWizardUpdate()
    {
        helpString = "Set the Character Mesh (your new prefab) and the Character reference (the player or enemy that you want to copy parameters)";
        if (CharacterMesh == null || CharacterReference == null)
        {
            errorString = "you should assign the characters";
            isValid = false;
        }
        else
        {
            errorString = "";
            isValid = true;
        }
    }

    void OnWizardCreate()
    {

        if (Type == CHR.Player)
        {
            CreatePlayer();
        }
        else if (Type == CHR.Enemy)
        {
            CreateEnemy();
        }

    }

    void CreatePlayer()

    {
        //Create new character and parent it
        GameObject characterGO = Instantiate(CharacterMesh, Vector3.zero, Quaternion.identity) as GameObject;
        characterGO.name = "Player";

        characterGO.tag = "Player";
        characterGO.layer = LayerMask.NameToLayer("Character");

        Transform rightHand = GameObject.Find(RightHandBone).transform as Transform;
        Debug.Log(rightHand.name);

        Transform leftHand = GameObject.Find(LeftHandBone).transform as Transform;
        Debug.Log(leftHand.name);

        //Look for Weapon Grip Node
        GameObject WeaponGrip = GameObject.Find("Weapon_R") as GameObject;
        if (WeaponGrip != null)
        {
            WeaponGrip.name = "Weapon_R_OLD";
            DestroyImmediate(WeaponGrip);
        }
  
        GameObject WeaponGripL = GameObject.Find("Weapon_L") as GameObject;
        if (WeaponGripL != null)
        {
            WeaponGripL.name = "Weapon_L_OLD";
            DestroyImmediate(WeaponGripL);
        }
  
        //New Player GO and replace reference
        GameObject newReference = Instantiate(CharacterReference, Vector3.zero, Quaternion.identity) as GameObject;
        newReference.name = CharacterMesh.name + "_Root";
        GameObject newReferencePlayer = newReference.GetComponentInChildren<PrTopDownCharController>().gameObject;
        newReferencePlayer.name = "OLD_Player";

        characterGO.transform.SetParent(newReference.transform);

         //Copy Components
        CopyComponents(newReferencePlayer.GetComponent<CapsuleCollider>(), characterGO);
        CopyComponents(newReferencePlayer.GetComponent<Rigidbody>(), characterGO);
        //DestroyImmediate(characterGO.GetComponent<Animator>());

        //AddComponents
        PrTopDownCharController newCharController = characterGO.AddComponent<PrTopDownCharController>();
        PrTopDownCharInventory newCharInv = characterGO.AddComponent<PrTopDownCharInventory>();
        AudioSource newAudio = characterGO.AddComponent<AudioSource>();

        CopyComponents(newReferencePlayer.GetComponent<PrTopDownCharController>(), newCharController);
        CopyComponents(newReferencePlayer.GetComponent<PrTopDownCharInventory>(), newCharInv);
        
        CopyComponents(newReferencePlayer.GetComponent<AudioSource>(), newAudio);
        Animator animator = characterGO.GetComponent<Animator>();
        animator.runtimeAnimatorController = AnimatorController as RuntimeAnimatorController;

        //Move Objects
        ReparentObjects(newReferencePlayer.transform.Find("PlayerLight"), characterGO.transform);
        ReparentObjects(newReferencePlayer.transform.Find("PlayerSelection"), characterGO.transform);
        ReparentObjects(newReferencePlayer.transform.Find("HUD"), characterGO.transform);
        ReparentObjects(newReferencePlayer.transform.Find("CameraTarget"), characterGO.transform);
        

        //Set New Skinned Meshes to inventory
        characterGO.GetComponent<PrTopDownCharInventory>().MeshRenderers = characterGO.GetComponentsInChildren<SkinnedMeshRenderer>();

        GameObject RefWeaponGrip = GameObject.Find("Weapon_R") as GameObject;
        Debug.Log(RefWeaponGrip.name);
        newCharInv.WeaponR = RefWeaponGrip.transform;

        GameObject aimIKNode = GameObject.Find("Weapon_IKAim") as GameObject;
        Debug.Log(aimIKNode.name);

        GameObject RefWeaponGripL = GameObject.Find("Weapon_L") as GameObject;
        Debug.Log(RefWeaponGripL.name);
        newCharInv.WeaponL = RefWeaponGripL.transform;

        ReparentObjects(aimIKNode.transform, rightHand.transform);
        ReparentObjects(RefWeaponGripL.transform, leftHand.transform);

        DestroyImmediate(newReferencePlayer);

        aimIKNode.transform.position = rightHand.transform.position;
        RefWeaponGripL.transform.position = leftHand.transform.position;

        Selection.activeGameObject = aimIKNode;

        //Debug.Log(WeaponGrip.name);
    }

    void CreateEnemy()
    {
        //Create new character and parent it
        GameObject characterGO = Instantiate(CharacterMesh, Vector3.zero, Quaternion.identity) as GameObject;
        characterGO.name = "Enemy_" + CharacterMesh.name;
        characterGO.tag = "Enemy";
        characterGO.layer = LayerMask.NameToLayer("Character");

        Animator animator = characterGO.GetComponent<Animator>();
        animator.runtimeAnimatorController = AnimatorController as RuntimeAnimatorController;
        GameObject WeaponGrip = new GameObject("Temp_Weapon_R");
        //Look for Weapon Grip Node
        if (GameObject.Find("Weapon_R"))
            WeaponGrip = GameObject.Find("Weapon_R").gameObject as GameObject;
        if (WeaponGrip != null)
        {
            if (GameObject.Find(RightHandBone))
            {
                WeaponGrip.transform.parent = GameObject.Find(RightHandBone).transform;
                WeaponGrip.transform.position = GameObject.Find(RightHandBone).transform.position;

            }
            else
            {
                WeaponGrip.transform.parent = characterGO.transform;
            }
            WeaponGrip.AddComponent<PrWeaponNodeHelper>();
            WeaponGrip.SetActive(false);
            Debug.Log("Weapon 1 has been Found " + WeaponGrip.name);

        }
            

        //New Player GO and replace reference
        GameObject newReferencePlayer = Instantiate(CharacterReference, Vector3.zero, Quaternion.identity) as GameObject;
        newReferencePlayer.name = CharacterReference.name + "_Reference";

        GameObject RefWeaponGrip = GameObject.Find("Weapon_R") as GameObject;
        AudioSource RefWeaponComp = GameObject.Find("Weapon_R").GetComponent<AudioSource>();
        if (WeaponGrip != null)
            WeaponGrip.SetActive(true);

        //AddComponents
        PrEnemyAI newEnemyAI = characterGO.AddComponent<PrEnemyAI>();
        AudioSource newAudio = characterGO.AddComponent<AudioSource>();

        //Copy Components
        CopyComponents(newReferencePlayer.GetComponent<PrEnemyAI>(), newEnemyAI);
        CopyComponents(newReferencePlayer.GetComponent<Rigidbody>(), characterGO);
        CopyComponents(newReferencePlayer.GetComponent<AudioSource>(), newAudio);
        CopyComponents(newReferencePlayer.GetComponent<CharacterController>(), characterGO);
        CopyComponents(newReferencePlayer.GetComponent<UnityEngine.AI.NavMeshAgent>(), characterGO);

        //Move Objects
        ReparentObjects(newReferencePlayer.transform.Find("DebugText"), characterGO.transform);

        //Set Weapon Grip
        if (WeaponGrip != null && RefWeaponGrip != null)
        {
            Debug.Log("Weapon Found");
            RefWeaponGrip.SetActive(false);

            CopyComponents(RefWeaponComp, WeaponGrip);
            newEnemyAI.WeaponGrip = WeaponGrip.transform;
            WeaponGrip.name = "Weapon_R";
        }
        else
        {
            Debug.Log("Weapon Not Found");
        }

        //Set Sensor Position
        GameObject EnemySensors = new GameObject("EnemySensor") as GameObject;
        EnemySensors.transform.parent = characterGO.transform;
        EnemySensors.transform.position = new Vector3(0, 1.7f, 0);
        newEnemyAI.eyesAndEarTransform = EnemySensors.transform;

        //Set New Skinned Meshes to inventory
        characterGO.GetComponent<PrEnemyAI>().MeshRenderers = characterGO.GetComponentsInChildren<SkinnedMeshRenderer>();

        DestroyImmediate(newReferencePlayer);
    }

    void CopyComponents(Component Source, Component Target )
    {
        ComponentUtility.CopyComponent(Source);
        ComponentUtility.PasteComponentValues(Target);
    }

    void CopyComponents(Component Source, GameObject Target)
    {
        ComponentUtility.CopyComponent(Source);
        ComponentUtility.PasteComponentAsNew(Target);
    }

    void ReparentObjects(Transform Target, Transform newParent)
    {
        Debug.Log("Reparent objects Target =" + Target.name + " New Parent = " + newParent);
        Target.parent = newParent;
    }
    
}