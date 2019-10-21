using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace CargaCotacoes
{
    public class CotacoesContext
    {
        private DocumentClient _client;

        public CotacoesContext(IConfiguration configuration)
        {
            _client = new DocumentClient(
                new Uri(configuration.GetSection("DBMoedas:EndpointUri").Value),
                configuration.GetSection("DBMoedas:PrimaryKey").Value);

            _client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = "DBMoedas" }).Wait();

            DocumentCollection collectionInfo = new DocumentCollection();
            collectionInfo.Id = "Cotacoes";

            collectionInfo.IndexingPolicy =
                new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

            _client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("DBMoedas"),
                collectionInfo,
                new RequestOptions { OfferThroughput = 400 }).Wait();
        }

        public void IncluirCotacao(object dadosCotacao)
        {
            _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(
                    "DBMoedas", "Cotacoes"), dadosCotacao).Wait();
        }
    }
}