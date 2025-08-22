using NUnit.Framework;
using UnityEngine;
using TL.EmotionalAI;

public class EmotionalAIPlayTests
{
    [Test]
    public void RelationshipAmplifier_RespectsClamp()
    {
        var d = new Vector3(1f, 1f, 1f); // exaggerated base
        var t = new Triangle{ I=1f, Pa=1f, C=1f };
        var outD = RelationshipAmplifier.Apply(d, t);
        Assert.That(outD.x, Is.InRange(-0.35f, 0.35f));
        Assert.That(outD.y, Is.InRange(-0.35f, 0.35f));
        Assert.That(outD.z, Is.InRange(-0.35f, 0.35f));
    }

    [Test]
    public void EmotionClassifier_8Octants_AreReachable()
    {
        var s = new PAD();
        s.P = 0.2f; s.A = 0.2f; s.D = 0.2f; Assert.AreEqual(EmotionOctant.Sad, EmotionClassifier.From(s));
        s.P = 0.8f; s.A = 0.2f; s.D = 0.2f; Assert.AreEqual(EmotionOctant.Tender, EmotionClassifier.From(s));
        s.P = 0.8f; s.A = 0.8f; s.D = 0.8f; Assert.AreEqual(EmotionOctant.Joy, EmotionClassifier.From(s));
        s.P = 0.2f; s.A = 0.8f; s.D = 0.8f; Assert.AreEqual(EmotionOctant.Angry, EmotionClassifier.From(s));
    }

    [Test]
    public void ApplyPlayerAction_ChangesPAD_AndStoresIntentAndDelta()
    {
        var go = new GameObject("npc");
        var model = go.AddComponent<EmotionModel>();
        var before = model.pad;
        model.ApplyPlayerAction(PlayerAction.KissQuick, 0.5f);
        Assert.AreNotEqual(before.P, model.pad.P);
        Assert.AreNotEqual(before.A, model.pad.A);
        Assert.AreEqual(Intent.Desire, model.lastIntent);
        Assert.That(model.lastDeltaApplied.magnitude, Is.GreaterThan(0f));
    }
}
