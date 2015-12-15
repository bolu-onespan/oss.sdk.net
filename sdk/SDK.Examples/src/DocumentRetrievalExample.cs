using System;
using Silanis.ESL.SDK;
using System.IO;
using Silanis.ESL.SDK.Builder;

namespace SDK.Examples
{
    public class DocumentRetrievalExample : SDKSample
    {
        public static void Main(string[] args)
        {
            new DocumentRetrievalExample(Props.GetInstance()).Run();
        }

        private string email1;
        private Stream fileStream1;

        public byte[] pdfDownloadedBytes, originalPdfDownloadedBytes, zippedDownloadedBytes;

        public DocumentRetrievalExample(Props props) : this(props.Get("api.key"), props.Get("api.url"), props.Get("1.email"))
        {
        }

        public DocumentRetrievalExample(String apiKey, String apiUrl, String email1) : base(apiKey, apiUrl)
        {
            this.email1 = email1;
            this.fileStream1 = File.OpenRead(new FileInfo(Directory.GetCurrentDirectory() + "/src/prêt.pdf").FullName);
        }

        override public void Execute()
        {
            String docId = "myDocumentId";
            var superDuperPackage = PackageBuilder.NewPackageNamed("DocumentRetrievalExample " + DateTime.Now)
                            .WithSigner(SignerBuilder.NewSignerWithEmail(email1)
                            .WithFirstName("George")
                            .WithLastName("Faltour").Build())
                    .WithDocument(DocumentBuilder.NewDocumentNamed("My Document")
                    .FromStream(fileStream1, DocumentType.PDF)
                            .WithId(docId)
                            .WithSignature(SignatureBuilder.SignatureFor(email1)
                           .    AtPosition(100, 100).OnPage(0))
                          ).Build();

            var packageId = eslClient.CreatePackageOneStep(superDuperPackage);

            eslClient.SendPackage(packageId);

            pdfDownloadedBytes = eslClient.DownloadDocument(packageId, docId);  
            originalPdfDownloadedBytes = eslClient.DownloadOriginalDocument(packageId, docId);
            zippedDownloadedBytes = eslClient.DownloadZippedDocuments(packageId);

            // To write the byte[] to a file, use:
            // System.IO.File.WriteAllBytes("/path/to/directory/myDocument.pdf", pdfDocumentBytes);
        }
    }
}
