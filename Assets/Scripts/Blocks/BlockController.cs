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
        CleanUp();
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

    public void CleanUp()
    {
        for(int i = 0 ; i < _blocks.Count; i++)
        {
            var i1 = i;
            _blocks[i].CleanUp();
            _blocks[i].OnClicked += () =>
            {
                OnBlockClicked?.Invoke(i1);
            };
        }
        
        SetAllBlockActive(false);
    }

    public void SetBlockColor(List<(int, int)> blocks, GameColor color)
    {
        foreach (var block in blocks)
        {
            int index = block.Item1 * 3 + block.Item2;
            _blocks[index].SetColor(color);
        }
    }
}
