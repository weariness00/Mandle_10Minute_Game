using GamePlay.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_CallingScreen : QuestBase
    {
        public CallingScreen calling;

        public override void OnNext(object value)
        {

        }
        public override void Play()
        {
            base.Play();
            var calls = PhoneUtil.InstantiateUI(calling, out var phone);
            phone.PhoneViewRotate(0);
            calls.name.text = "mom";
            calls.ClearAction += Complete;
            calls.IgnoreAction += Ignore;
        }
        public override void Complete()
        {
            base.Complete();
        }

        public override void Ignore()
        {
            base.Ignore();
        }
    }
}