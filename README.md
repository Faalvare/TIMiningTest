El proyecto esta desarrollado en Unity 2020.3.3f1 LTS , cuenta con controles para observar escena, los controles estan basados en el programa Autodesk 3ds Max.

## Controles:
- Botón central del mouse : Movimiento de paneo.
- Botón central del mouse + alt : Rotacion de la camara.
- Rueda del mouse o ctrl + alt + boton central del mouse : Zoom.
- Escape para cerrar la aplicación.

## Graficos
Para los graficos se usa Universal Render pipeline la escena viene con efectos de post procesado y hay dos shaders creados en el ShaderGraph de unity
### Shaders
- StandardUrpLitShader : Es un shader muy parecido al lit que viene con universal render pipeline y se usa para aplicarlo en objetos que son importados por codigo.
- FresnelGlow : Un shader que genera un efecto fresnel que se aplica al pasar el mouse por encima de las palas, viene con la propiedad de poder cambiar la frecuencia e intensidad.
## Assets usados
- 2D Sprite Package desde package manager
- Simple UI Elements : https://assetstore.unity.com/packages/2d/gui/icons/simple-ui-elements-53276
- Iconos de mouse click : https://www.vecteezy.com/vector-art/104366-mouse-instruction-vectors
- Modelo de pala : https://assetstore.unity.com/packages/3d/props/realistic-shovel-clean-179431
- Json.net : https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347
- Async await Support :  https://assetstore.unity.com/packages/tools/integration/async-await-support-101056
- Simple Button Set : https://assetstore.unity.com/packages/2d/gui/icons/simple-button-set-02-184903
- Controles de camara : http://wiki.unity3d.com/index.php?title=MouseOrbitZoom