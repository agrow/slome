using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TL.EmotionalAI
{
    // Attach this to any GameObject in a test scene. Assign references in Inspector.
    public class EmotionalAIDebugHUD : MonoBehaviour
    {
        [Header("Refs")]
        public EmotionModel model;      // NPC's EmotionModel
        public EmotionBrain brain;      // NPC's EmotionBrain
        public EmotionAdapter adapter;  // NPC's EmotionAdapter (optional)

        [Header("UI")]
        public bool autoAct = true;
        [Range(0f,1f)] public float intensity01 = 0.5f;
        Vector2 _scroll;

        static readonly PlayerAction[] _actions = (PlayerAction[])Enum.GetValues(typeof(PlayerAction));

        void OnGUI()
        {
            const int w = 430;
            GUILayout.BeginArea(new Rect(10, 10, w, Screen.height - 20), GUI.skin.box);
            GUILayout.Label("<b>EmotionalAI Debug HUD</b>", new GUIStyle(GUI.skin.label){richText=true, fontSize=14});

            // Relationship sliders
            GUILayout.Space(6);
            GUILayout.Label("<b>Triangle (Relationship)</b>", Rich());
            model.tri.I = Slider("Intimacy (I)", model.tri.I);
            model.tri.Pa = Slider("Passion (Pa)", model.tri.Pa);
            model.tri.C = Slider("Commitment (C)", model.tri.C);

            // PAD readout
            GUILayout.Space(6);
            GUILayout.Label("<b>PAD</b>", Rich());
            GUILayout.Label($"P: {model.pad.P:0.00}   A: {model.pad.A:0.00}   D: {model.pad.D:0.00}");
            GUILayout.Label($"Last Δ: ({model.lastDeltaApplied.x:0.00}, {model.lastDeltaApplied.y:0.00}, {model.lastDeltaApplied.z:0.00})");
            GUILayout.Label($"Emotion: <b>{model.lastEmotion}</b>   Intent: {model.lastIntent}", Rich());

            // Intensity
            GUILayout.Space(6);
            intensity01 = Slider("Intensity (0..1)", intensity01);

            // PlayerAction buttons
            GUILayout.Space(6);
            GUILayout.Label("<b>Fire Player Action</b>", Rich());
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(160));
            int col = 2; int i=0;
            GUILayout.BeginHorizontal();
            foreach (var a in _actions)
            {
                if (GUILayout.Button(a.ToString(), GUILayout.Width((w-40)/col)))
                {
                    if (adapter != null)
                    {
                        adapter.OnPlayerAction(a, intensity01);
                    }
                    else
                    {
                        model.ApplyPlayerAction(a, intensity01);
                        brain.DecideBestEmotionalAction();
                        if (autoAct) brain.ExecuteBest();
                    }
                }
                if (++i % col == 0) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            // Action utilities
            GUILayout.Space(6);
            GUILayout.Label("<b>Emotional Action Scores</b>", Rich());
            if (brain != null)
            {
                // Score without executing to show live values
                var list = new List<(EmotionalAction act, float score)>();
                foreach (var act in brain.GetType().GetField("actions", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance)?.GetValue(brain) as EmotionalAction[] ?? Array.Empty<EmotionalAction>())
                {
                    float s = act.ScoreAction(model);
                    list.Add((act, s));
                }
                foreach (var row in list.OrderByDescending(r => r.score))
                {
                    GUILayout.Label($"{row.act.Name}: {row.score:0.000}");
                }
            }

            // Controls
            GUILayout.Space(6);
            autoAct = GUILayout.Toggle(autoAct, "Auto Execute Best");
            if (GUILayout.Button("Decide & Execute Now")) { brain.DecideBestEmotionalAction(); brain.ExecuteBest(); }
            if (GUILayout.Button("Reset PAD to Neutral (0.5)"))
            {
                model.pad = new PAD{P=0.5f, A=0.5f, D=0.5f};
                model.lastDeltaApplied = Vector3.zero;
                model.lastEmotion = EmotionClassifier.From(model.pad);
            }

            GUILayout.EndArea();
        }

        float Slider(string label, float v)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(170));
            v = GUILayout.HorizontalSlider(v, 0f, 1f);
            GUILayout.Label(v.ToString("0.00"), GUILayout.Width(40));
            GUILayout.EndHorizontal();
            return Mathf.Clamp01(v);
        }
        GUIStyle Rich() => new GUIStyle(GUI.skin.label){richText=true};
    }
}
