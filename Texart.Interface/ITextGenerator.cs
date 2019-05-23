﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Texart.Interface
{
    /// <summary>
    /// An <code>ITextGenerator</code> is used to generate <code>ITextData</code>s given
    /// some configuration options.
    /// </summary>
    /// <see cref="ITextData"/>
    /// <see cref="ITextGenerator"/>
    public interface ITextGenerator
    {
        /// <summary>
        /// Gets the available characters that will be used in the generated text.
        /// This listed shall be sorted from brightest characters to darkest characters.
        /// The definitions of dark and bright are implementation-defined.
        /// </summary>
        IList<char> Characters { get; }

        /// <summary>
        /// Gets the ratio of source resolution to generated resolution. That is, one
        /// pixel in the generated text will come from sampling <code>Math.Pow(PixelSamplingRatio, 2)</code>
        /// pixels.
        /// For example, if <code>PixelSamplingRatio</code> is <code>2</code>, then <code>4</code>
        /// pixels from the image will be used to generate <code>1</code> character.
        /// Consequently, a value of <code>1</code> is loss-less.
        /// </summary>
        /// <see cref="GenerateTextAsync"/>
        int PixelSamplingRatio { get; }

        /// <summary>
        /// Generates a <code>ITextData</code> with the <code>PixelSamplingRatio</code> adjusted dimensions
        /// and using the available characters in <code>Characters</code>.
        /// </summary>
        /// <param name="bitmap">The bitmap to generate data from.</param>
        /// <returns></returns>
        /// <see cref="PixelSamplingRatio"/>
        /// <see cref="Characters"/>
        Task<ITextData> GenerateTextAsync(Bitmap bitmap);
    }
}
