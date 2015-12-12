﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ArrowSyncC_BHV : MonoBehaviour, SyncableObject{

    public const int SYNC_TYPE = 2;

    private int index = -1;
    private ArrowC_BHV realObj;

    public static SyncableObject CreateSyncableFromMessage(int index, SendMode mode, int mask, BinaryReader reader)
    {
        SyncableObject obj = CreateNew(index);
        obj.ReadFromBuffer(reader, mode, mask);
        return obj;
    }

    // Create and initializes the object
    protected static ArrowSyncC_BHV CreateNew(int index)
    {
        GameObject g = Instantiate(GameData.FindObjectOfType<GameData>().Arrow);
        ArrowSyncC_BHV comp = g.GetComponent<ArrowSyncC_BHV>();
        comp.index = index;
        return comp;
    }



    // Initialize basic variables
    public void Awake()
    {
        realObj = GetComponent<ArrowC_BHV>();
    }

    public int CalculateDataSize(SendMode mode, int mask)
    {
        int s = 0;

        s += DataWriter.Vector3Size();
        s += DataWriter.Vector3Size();

        return (ushort)s;
    }

    public int getIndex()
    {
        return index;
    }

    public void ReadFromBuffer(BinaryReader buffer, SendMode mode, int mask)
    {
        float ping = 16/1000;
        RaycastHit Hit;
        Vector3 p = DataReader.ReadVector3(buffer);
        Vector3 v = DataReader.ReadVector3(buffer);

        // Calculate new position based on ping
        Vector3 newp = p + v * ping + Vector3.down * ((9.8f * ping * ping) / 2.0f);
        Vector3 newv = v + Vector3.down * (9.8f * ping);
        Vector3 direction = (newp - p);

        if (Physics.Raycast(p - 0.01f*direction, direction, out Hit, direction.magnitude, realObj.CollisionMask))
        {
            newp = Hit.point;
            realObj.MakeStuck(newp, newv);
        }

        realObj.Position = newp;
        realObj.Velocity = newv;
    }
}