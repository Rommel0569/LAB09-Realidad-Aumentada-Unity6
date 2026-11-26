# LAB09 Realidad Aumentada Unity 6

Proyecto del Laboratorio 09 de Desarrollo de Software para Juegos. La escena combina un sistema solar navegable y un personaje animado, reconocidos mediante dos Image Targets de Vuforia.

## Requisitos

- Unity Editor `6000.3.17f1` (Unity 6.3 LTS).
- Módulo Android Build Support con Android SDK & NDK Tools y OpenJDK.
- Vuforia Engine `11.4.4` para Unity.
- Una App License Key propia de Vuforia para inicializar la cámara y rastrear imágenes.

## Clonar e instalar Vuforia

1. Clona el repositorio y descarga Vuforia Engine 11.4.4 para Unity desde el [portal oficial de descargas](https://developer.vuforia.com/downloads/sdk). Se requiere una cuenta de Vuforia y aceptar el acuerdo de desarrollador.
2. Para instalar el paquete, usa una de estas opciones:
   - Importa `add-vuforia-package-11-4-4.unitypackage` en Unity mediante **Assets > Import Package > Custom Package** y acepta la instalación de Vuforia Engine 11.4.4.
   - O coloca el tarball `com.ptc.vuforia.engine-11.4.4.tgz` dentro de `Packages/`. El `Packages/manifest.json` ya apunta a ese nombre.
3. Abre en Unity Hub la carpeta raíz clonada. Espera a que Unity termine de resolver los paquetes e importar los assets.
4. En Unity, abre **Window > Vuforia Configuration** y pega tu clave en **App License Key**. El repositorio deja el campo vacío intencionalmente; no publica ninguna clave personal.

El tarball de Vuforia no se incluye porque tiene aproximadamente 138 MB, supera el límite de 100 MB por archivo de GitHub y Vuforia lo distribuye desde su portal. La guía oficial también documenta la instalación desde el paquete Unity o desde el tarball: [Vuforia Engine Package for Unity](https://developer.vuforia.com/library/vuforia-engine/unity-extension/vuforia-engine-package-unity/).

## Abrir y probar

1. Abre `Assets/Scenes/SampleScene.unity`.
2. Usa el marcador `Assets/Solar System/ImageTargets/SolarSystem.jpg` para ver el sistema solar. Los botones **Previo** y **Siguiente** recorren Sol, Mercurio, Tierra, Venus, Marte, Júpiter, Saturno, Urano y Neptuno.
3. Usa `Assets/Solar System/ImageTargets/AlienMarker.png` para mostrar a Astro Amigo. Al reconocerlo aparecen **Caminar**, **Saltar** y **Aplaudir**.
4. Imprime los marcadores con buena calidad, sin recortarlos ni distorsionar su proporción. La cámara necesita iluminación suficiente y una vista clara del marcador.

## Generar APK Android

1. Conecta o configura Android desde **File > Build Profiles** y activa Android.
2. En **Project Settings > Player > Other Settings**, usa API mínima 29 o superior y conserva la configuración de entrada **Input Manager (Old)** de este proyecto. Si Vulkan figura en Graphics APIs, elimínalo para mantener la configuración usada durante la validación.
3. Verifica que la clave de Vuforia esté pegada en **Vuforia Configuration**.
4. Abre **File > Build Profiles**, confirma que `Assets/Scenes/SampleScene.unity` esté incluida y selecciona **Build** o **Build And Run**.

## Archivos principales

- `Assets/Scenes/SampleScene.unity`: escena de la aplicación.
- `Assets/Scripts/`: creación de targets, navegación de planetas, animaciones y controles contextuales.
- `Assets/Resources/AnimatedAlien.fbx`: modelo y clips de animación.
- `Assets/Resources/VuforiaConfiguration.asset`: configuración con el campo de licencia vacío para proteger la clave.
- `Assets/Solar System/ImageTargets/`: imágenes que se imprimen y se enfocan con la cámara.
- `Docs/Informe_Laboratorio_Realidad_Aumentada.pdf`: informe del laboratorio.

Las carpetas generadas por Unity (`Library`, `Temp`, `Logs`, `obj` y `UserSettings`) y los APK no se versionan; Unity las regenera localmente.
