using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour, IFocusable
{
    public Molecule molecule;
    public AtomicBond[]   bondingGroups;
    public int      atomicNumber;       // note: 0 stands in for pure electron groups
    public float    atomicMass;
    public int      freeElectrons;
    public int      missingElectrons;
    public int      atomicGroup;
    public Color    color;
    public int maxBondingGroups;
    public int currentBondingGroups;

    public void SetAtomType (AtomicTemplate template)
    {
        this.atomicNumber = template.atomicNumber;
        this.atomicMass = template.mass;
        this.color = template.color;
        this.atomicGroup = 0;

        transform.localScale = Vector3.one * 0.1f * template.radius;

        // calculate electron groups...
        if (atomicNumber <= 0) return;

        freeElectrons = atomicNumber;
        var activeShellSize = 0;
        while (freeElectrons > 0)
        {
            ++atomicGroup;
            activeShellSize = 2;
            if (freeElectrons <= 2) { missingElectrons = 2 - freeElectrons;  break; } else { freeElectrons -= 2;  }
            if (atomicGroup >= 4) { activeShellSize = 5; if (freeElectrons <= 10) { missingElectrons = 10 - freeElectrons; break; } else { freeElectrons -= 10; } }
            if (atomicGroup >= 2) { activeShellSize = 4; if (freeElectrons <= 6) { missingElectrons = 8 - freeElectrons; break; } else { freeElectrons -= 6; } }
        }
        maxBondingGroups = currentBondingGroups = activeShellSize;
        bondingGroups = new AtomicBond[activeShellSize];

        ShowByColor();
        SetupDefaultElectronGroups();
    }
    public void ShowByColor ()
    {
        GetComponent<Renderer>().material.color = color;
    }
    void SetupDefaultElectronGroups()
    {
        int n = freeElectrons;
        currentBondingGroups = maxBondingGroups;
        for (int i = 0; i < maxBondingGroups && n-- > 0; ++i) {
            if (bondingGroups[i] == null)
                bondingGroups[i] = AtomicBond.MakeElectronGroup();
        }
        if (n > 0)
        {
            for (int i = 0; i < maxBondingGroups && n --> 0; ++i)
            {
                bondingGroups[i].SetElectronCount(2);
                freeElectrons -= 2;
                --maxBondingGroups;
            }
        }
    }
    void ClearElectronBondingGroups()
    {
        for (int i = 0; i < maxBondingGroups; ++i)
        {
            if (bondingGroups[i].isElectronGroup)
            {
                freeElectrons += bondingGroups[i].electronCount;
            }
            bondingGroups[i] = null;
        }
    }
    public bool TryAddBond(Atom other, int bondStrength = 1) {
        if (freeElectrons >= bondStrength && currentBondingGroups > 0 && other.freeElectrons >= bondStrength && other.maxBondingGroups > 0)
        {
            freeElectrons -= bondStrength;
            other.freeElectrons -= bondStrength;
            ClearElectronBondingGroups();
            bool ok = false;
            for (int i = 0; i < maxBondingGroups; ++i)
            {
                for (int j = 0; j < other.maxBondingGroups; ++j)
                {
                    if (bondingGroups[i] == null && other.bondingGroups[j] == null)
                    {
                        bondingGroups[i] = bondingGroups[j] = AtomicBond.Create(this, other);
                        ok = true;
                        break;
                    }

                }
            }
            if (!ok)
            {
                freeElectrons += bondStrength;
                other.freeElectrons += bondStrength;
            }
            SetupDefaultElectronGroups();
            return ok;
        }
        return false;
    }

    void IFocusable.OnSetFocused(bool focused)
    {
        if (focused)
        {
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
        else
        {
            GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
