using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileIndicators
{
    public class MovementIndicator : TileBase
    {
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Resources.Load<Sprite>("Sprites/Tiles/Indicators/movementTileIndicator");
        }
    }

    public class AggroIndicator : TileBase
    {
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Resources.Load<Sprite>("Sprites/Tiles/Indicators/aggroTileIndicator");
        }
    }

    public class EncounterIndicator : TileBase
    {
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Resources.Load<Sprite>("Sprites/Tiles/Indicators/encounterTileIndicator");
        }
    }

    public class LoadingZoneIndicator : TileBase
    {
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Resources.Load<Sprite>("Sprites/Tiles/Indicators/loadingZoneTileIndicator");
        }
    }

    public class AllyTargetingSkillIndicator : TileBase
    {
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Resources.Load<Sprite>("Sprites/Tiles/Indicators/allyTargetingSkillTileIndicator");
        }
    }

    public class EnemyTargetingSkillIndicator : TileBase
    {
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Resources.Load<Sprite>("Sprites/Tiles/Indicators/enemyTargetingSkillTileIndicator");
        }
    }

    public class HitboxTile : TileBase
    {
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Resources.Load<Sprite>("Sprites/Tiles/Prototype/hitbox");
        }
    }


}