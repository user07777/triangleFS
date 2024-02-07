open System
open FsGL
open OpenTK.Graphics.OpenGL

type ShaderType =
    | VertexShader
    | FragmentShader

let compileShader (shaderType: ShaderType) (source: string) : int =
    let shader = GL.CreateShader(Enum.Parse<ShaderType>(shaderType.ToString()))
    GL.ShaderSource(shader, source)
    GL.CompileShader(shader)
    let result = 0
    GL.GetShader(shader, ShaderParameter.CompileStatus, &result)
    if result = 0 then
        let log = GL.GetShaderInfoLog(shader)
        Console.WriteLine(log)
    shader

let linkProgram (vertexShader: int) (fragmentShader: int) : int =
    let program = GL.CreateProgram()
    GL.AttachShader(program, vertexShader)
    GL.AttachShader(program, fragmentShader)
    GL.LinkProgram(program)
    program

let vertexShaderSource = "
    #version 330 core
    layout (location = 0) in vec3 inPosition;
    void main()
    {
        gl_Position = vec4(inPosition, 1.0);
    }"

let fragmentShaderSource = "
    #version 330 core
    out vec4 FragColor;
    void main()
    {
        FragColor = vec4(1.0, 0.5, 0.2, 1.0);
    }"

[<EntryPoint>]
let main argv =
    let window = new GlutWindow()
    window.Title <- "OpenGL with F#"

    let vertexShader = compileShader VertexShader vertexShaderSource
    let fragmentShader = compileShader FragmentShader fragmentShaderSource
    let shaderProgram = linkProgram vertexShader fragmentShader

    let vertices : float[] = [|
        -0.5f; -0.5f; 0.0f;
         0.5f; -0.5f; 0.0f;
         0.0f;  0.5f; 0.0f |]

    let vao = GL.GenVertexArray()
    let vbo = GL.GenBuffer()
    GL.BindVertexArray(vao)
    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo)
    GL.BufferData(BufferTarget.ArrayBuffer, sizeof<float> * vertices.Length, vertices, BufferUsageHint.StaticDraw)
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof<float>, 0)
    GL.EnableVertexAttribArray(0)
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
    GL.BindVertexArray(0)

    let init () =
        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f)
        GL.ShadeModel(ShadingModel.Flat)

    let display () =
        GL.Clear(ClearBufferMask.ColorBufferBit)

        GL.UseProgram(shaderProgram)
        GL.BindVertexArray(vao)
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3)

        window.SwapBuffers()

    window.InitFunction <- init
    window.DisplayFunc <- display

    window.Run()

    0
