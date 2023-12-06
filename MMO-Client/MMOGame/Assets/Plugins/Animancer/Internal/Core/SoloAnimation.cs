// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2023 Kybernetik //

using Animancer.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Animancer
{
    /// <summary>Plays a single <see cref="AnimationClip"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/playing/component-types">Component Types</see>
    /// </remarks>
    /// <example>
    /// <see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/solo-animation">Solo Animation</see>
    /// </example>
    /// https://kybernetik.com.au/animancer/api/Animancer/SoloAnimation
    /// 
    [AddComponentMenu(Strings.MenuPrefix + "Solo Animation")]
    [DefaultExecutionOrder(DefaultExecutionOrder)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(SoloAnimation))]
    public class SoloAnimation : SoloAnimationInternal { }
}
