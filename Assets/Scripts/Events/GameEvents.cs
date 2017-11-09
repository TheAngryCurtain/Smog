﻿using UnityEngine;

public enum eGameState { InGame, Customization };

public class GameEvents
{
    public class GameStateChangedEvent : VSGameEvent
    {
        public eGameState State;

        public GameStateChangedEvent(eGameState state)
        {
            State = state;
        }
    }

    public class PaintRequestAtPositionEvent : VSGameEvent
    {
        public Vector2 Position;

        public PaintRequestAtPositionEvent(Vector2 pos)
        {
            Position = pos;
        }
    }
}