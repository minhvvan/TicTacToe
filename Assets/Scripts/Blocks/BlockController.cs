using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private List<Block> _blocks;

    public Action<int> OnBlockClicked;

    public void Awake()
    {
        _blocks = GetComponentsInChildren<Block>().ToList();

        for(int i = 0 ; i < _blocks.Count; i++)
        {
            var i1 = i;
            _blocks[i].Clear();
            _blocks[i].OnClicked += () =>
            {
                OnBlockClicked?.Invoke(i1);
            };
        }
    }

    public void PlaceMaker(PlayerType type, int index)
    {
        _blocks[index].SetMaker(type);
    }

    public void SetAllBlockActive(bool bActive)
    {
        foreach (var block in _blocks)
        {
            block.IsActive = bActive;
        }
    }
}
