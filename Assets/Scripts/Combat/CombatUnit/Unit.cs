using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using System.Linq;
using DG.Tweening;

namespace PsychOutDestined
{
    public enum UnitClass
    {
        None = -1,

        Other = 5
    }

    public partial class Unit : MonoBehaviour
    {
        // Unit properties
        [Header("Vanity")]
        public string UnitName;
        [SerializeField] protected UnitAnimator unitAnimator;
        [SerializeField] protected SpriteRenderer unitSprite;
        protected Sequence currentBlinkCo;

        protected const float BLINK_TIME = 1f;

        [Header("Runtime")]
        public CombatAction currentSelectedAction;
        public List<Unit> selectedTargets;

        [HideInInspector] public bool isUnitActive = true;

        [HideInInspector] public int lastSelectedTurnMenuOptionIndex = 0;
        [HideInInspector] public int lastSelectedActionMenuOptionIndex = 0;

        protected Color BLINK_MIN_COLOR;

        // Unit animator
        //public UnitAnimator unitAnimator;

        // Initialization
        protected virtual void Awake()
        {
            BLINK_MIN_COLOR = new Color(1, 1, 1, .1f);
            InitializeUnit();
        }

        protected virtual void InitializeUnit()
        {
            unitStats = new Dictionary<UnitStat, int>();
            InitializeStats();

            OnHPChanged += CheckUnitStatus;

            unitAnimator.SetAnimationState(UnitAnimationState.Idle);

            OnUnitInitialized?.Invoke();
        }

        public delegate void OnUnitInitializedDelegate();
        public event OnUnitInitializedDelegate OnUnitInitialized;

        protected void CheckUnitStatus(int newStatValue = -1)
        {
            if (HP <= 0)
            {
                isUnitActive = false;
                Debug.Log($"{name} is defeated!");
            }
        }

        public override string ToString()
        {
            return $"Name: {name}\nHP: {HP}";
        }

        public void SetUnitAnimatorState(UnitAnimationState state) => unitAnimator.SetAnimationState(state);

        public void StartBlinking()
        {
            if (currentBlinkCo == null || !currentBlinkCo.IsPlaying())
            {
                float halfBlinkTime = BLINK_TIME / 2;
                currentBlinkCo = DOTween.Sequence();
                currentBlinkCo.Append(unitSprite.DOColor(BLINK_MIN_COLOR, halfBlinkTime).SetEase(Ease.InCubic));
                currentBlinkCo.Append(unitSprite.DOColor(Color.white, halfBlinkTime).SetEase(Ease.OutCubic));
                currentBlinkCo.SetLoops(-1, LoopType.Restart);

                unitSprite.color = Color.white;
                currentBlinkCo.Play();
            }
        }

        public void StopBlinking()
        {
            if (currentBlinkCo != null && currentBlinkCo.IsPlaying())
            {
                currentBlinkCo.Kill(true);
                currentBlinkCo = null;
                unitSprite.color = Color.white;
            }
        }

        public void MoveUnit(Vector3 position)
        {
            transform.position = position;
            OnUnitMoved?.Invoke();
        }
        public delegate void OnUnitMovedDelegate();
        public event OnUnitMovedDelegate OnUnitMoved;

        protected void HandleOnUnitInitialized()
        {
            OnUnitInitialized?.Invoke();
        }
    }
}