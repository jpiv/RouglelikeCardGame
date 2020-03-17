using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour {
	public GameObject lootContainer;

    void Start() {
		this.GenerateLoot(); 
    }

    void GenerateLoot() {
    	this.GenerateBones();
    	this.GenerateSouls();
        this.GenerateExperince();
    }

    void GenerateBones() {
    	int bones = Random.Range(0, 21);
    	string bonesText = $"Bones x{ bones }";
    	Creator.CreateLootText(bonesText, this.lootContainer.transform);
    	Store.AddBones(bones);
    }

    void GenerateSouls() {
    	int souls = Random.Range(0, 21);
    	string soulsText = $"Souls x{ souls }";
    	Creator.CreateLootText(soulsText, this.lootContainer.transform);
    	Store.AddSouls(souls);
    }

    void GenerateExperince() {
        int exp = 110;
        string expText = $"EXP x{ exp }";
        Creator.CreateLootText(expText, this.lootContainer.transform);
        Store.AddExp(exp);
    }
}
