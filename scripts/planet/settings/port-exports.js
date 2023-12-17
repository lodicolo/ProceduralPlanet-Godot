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
        ...sortedExports.map(({ type, defaultValue, name }) => `private ${type} _${name.replace(/^./, m => m.toLowerCase())}${(defaultValue ? `= ${defaultValue}` : '')};`),
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

console.log(convertExports(
    [
        { type: 'int', name: 'TextureSize', defaultValue: '256' },
        { type: 'int', name: 'InScatteringPoints', defaultValue: '10' },
        { type: 'int', name: 'OpticalDepthPoints', defaultValue: '10' },
        { type: 'float', name: 'DensityFalloff', defaultValue: '0.25f' },
        { type: 'Vector3', name: 'Wavelengths', defaultValue: 'new Vector3(700, 530, 460)' },
        { type: 'float', name: 'ScatteringStrength', defaultValue: '20' },
        { type: 'float', name: 'Intensity', defaultValue: '1' },
        { type: 'float', name: 'DitherStrength', defaultValue: '0.8f' },
        { type: 'float', name: 'DitherScale', defaultValue: '4' },
        { type: 'float', name: 'AtmosphereScale', defaultValue: '0.5f' },
        { type: 'Texture2D?', name: 'BlueNoiseTexture' },
        { type: 'ImageTexture?', name: 'OpticalDepthTexture' },
    ],
    true
));