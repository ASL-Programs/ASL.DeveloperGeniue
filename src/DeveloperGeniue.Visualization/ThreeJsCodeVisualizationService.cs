using System.Diagnostics;

namespace DeveloperGeniue.Visualization;

/// <summary>
/// Basic yet functional Three.js-based code visualization implementation.
/// The service generates an HTML page containing a minimal Three.js scene that
/// visualizes each <c>.cs</c> file in the project directory as a cube.  The page
/// is written to a temporary file and opened in the default browser.
/// </summary>
public class ThreeJsCodeVisualizationService : ICodeVisualizationService
{
    public async Task RenderAsync(string projectPath)
    {
        var files = Directory
            .EnumerateFiles(projectPath, "*.cs", SearchOption.AllDirectories)
            .Select(Path.GetFileName)
            .ToList();

        var json = System.Text.Json.JsonSerializer.Serialize(files);
        var html = @$"<html><head>
    <script src='https://cdn.jsdelivr.net/npm/three@0.161.0/build/three.min.js'></script>
</head>
<body style='margin:0'>
<canvas id='c'></canvas>
<script>
const files = {json};
const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(75, window.innerWidth/window.innerHeight, 0.1, 1000);
const renderer = new THREE.WebGLRenderer({{canvas: document.getElementById('c')}});
renderer.setSize(window.innerWidth, window.innerHeight);
camera.position.z = 5;
files.forEach((f, i) => {{
  const geometry = new THREE.BoxGeometry(0.5,0.5,0.5);
  const material = new THREE.MeshNormalMaterial();
  const cube = new THREE.Mesh(geometry, material);
  cube.position.x = (i % 10) - 5;
  cube.position.y = Math.floor(i / 10);
  scene.add(cube);
}});
function animate() {{
  requestAnimationFrame(animate);
  scene.rotation.y += 0.01;
  renderer.render(scene, camera);
}}
animate();
</script>
</body></html>";

        var tempFile = Path.Combine(Path.GetTempPath(), "devgen_threejs.html");
        await File.WriteAllTextAsync(tempFile, html);
        Process.Start(new ProcessStartInfo { FileName = tempFile, UseShellExecute = true });
    }
}
