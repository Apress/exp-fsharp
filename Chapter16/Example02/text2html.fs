#light
open System.IO
open System.Text

let main() = 
   if Sys.argv.Length <= 2 then
         let base = Path.GetFileName(Sys.argv.[0])
         eprintfn "Usage: %s dir pattern" base;
         exit 1
   let directory = Sys.argv.[1]
   let pattern = Sys.argv.[2]

   for fileName in Directory.GetFiles(directory, pattern) do
      
      // Open a file stream for the file name
      use inputReader = File.OpenText(fileName)
      
      // Create a lex buffer for use with the generated lexer. The lex buffer
      // reads the inputReader stream.
      let lexBuffer = Lexing.from_text_reader Encoding.ASCII inputReader
      
      // Open an output channel
      let outputFile = Path.ChangeExtension(fileName,"html")
      use outputWriter = (new StreamWriter(outputFile) :> TextWriter)
      
      // Write the header
      fprintfn outputWriter "<html>\n<head></head>\n<pre>"
      
      // Run the generated lexer
      Text2htmllex.convertHtml outputWriter lexBuffer
      
      // Write the footer
      fprintfn outputWriter "</pre>\n</html>\n"

do main()
 
