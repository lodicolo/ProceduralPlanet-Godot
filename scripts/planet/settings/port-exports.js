/**
 * @typedef {{ type, defaultValue, name }} ExportDecl
 */
/**
 *
 * @param {ExportDecl[]} exports
 * @returns {string}
 */
function convertExports(exports, dirty) {
    const sortedExports = exports.sort(({ name: a }, { name: b }) => a.localeCompare(b));
    return [
        ...sortedExports.map(({ type, defaultValue, name }) => `\tprivate ${type} _${name.replace(/^./, m => m.toLowerCase())}${(defaultValue ? ` = ${defaultValue}` : '')};`),
        ...sortedExports.map(({ type, name}) => `
        [Export] public ${type} ${name}
        {
            get => _${name.replace(/^./, m => m.toLowerCase())};
            set
            {
                if (SetIfChanged(ref _${name.replace(/^./, m => m.toLowerCase())}, value${(dirty ? ', ref _dirty' : '')}))
                {
                    EmitChanged();
                }
            }
        }`)
    ].join('\n');
}

// AtmosphereSettings
// console.log(convertExports(
//     [
//         { type: 'int', name: 'TextureSize', defaultValue: '256' },
//         { type: 'int', name: 'InScatteringPoints', defaultValue: '10' },
//         { type: 'int', name: 'OpticalDepthPoints', defaultValue: '10' },
//         { type: 'float', name: 'DensityFalloff', defaultValue: '0.25f' },
//         { type: 'Vector3', name: 'Wavelengths', defaultValue: 'new Vector3(700, 530, 460)' },
//         { type: 'float', name: 'ScatteringStrength', defaultValue: '20' },
//         { type: 'float', name: 'Intensity', defaultValue: '1' },
//         { type: 'float', name: 'DitherStrength', defaultValue: '0.8f' },
//         { type: 'float', name: 'DitherScale', defaultValue: '4' },
//         { type: 'float', name: 'AtmosphereScale', defaultValue: '0.5f' },
//         { type: 'Texture2D?', name: 'BlueNoiseTexture' },
//         { type: 'ImageTexture?', name: 'OpticalDepthTexture' },
//     ],
//     true
// ));

// OceanSettings
console.log(convertExports(
    [
        { type: 'float', name: 'AlphaMultiplier', defaultValue: '10' },
        { type: 'float', name: 'DepthMultiplier', defaultValue: '70' },
        { type: 'Color', name: 'ColorA' },
        { type: 'Color', name: 'ColorB' },
        { type: 'Color', name: 'SpecularColor', defaultValue: 'new Color(1f, 1f, 1f)' },
        { type: 'Texture2D?', name: 'WaveNormalA' },
        { type: 'Texture2D?', name: 'WaveNormalB' },
        { type: 'float', name: 'WaveStrength', defaultValue: '0.15f', range: '0,1' },
        { type: 'float', name: 'WaveScale', defaultValue: '15' },
        { type: 'float', name: 'WaveSpeed', defaultValue: '0.5f' },
        { type: 'float', name: 'ShoreWaveHeight', defaultValue: '0.1f' },
        { type: 'float', name: 'Smoothness', defaultValue: '0.92f', range: '0,1' },
        { type: 'float', name: 'SpecularIntensity', defaultValue: '0.5f' },
        { type: 'Texture2D?', name: 'FoamNoiseTexture' },
        { type: 'Color', name: 'FoamColor' },
        { type: 'float', name: 'FoamNoiseScale', defaultValue: '1' },
        { type: 'float', name: 'FoamFalloffDistance', defaultValue: '0.5f' },
        { type: 'float', name: 'FoamEdgeFalloffBias', defaultValue: '0.5f' },
        { type: 'float', name: 'FoamLeadingEdgeFalloffBias', defaultValue: '1' },
        { type: 'float', name: 'RefractionScale', defaultValue: '1', range: '0,2' },
    ],
    true
));