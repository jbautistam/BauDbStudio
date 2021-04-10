using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.RestStudio.Models;
using Bau.Libraries.RestStudio.Models.Rest;

namespace Bau.Libraries.RestStudio.Application.Repositories
{	
	/// <summary>
	///		Repositorio de <see cref="RestSolutionModel"/>
	/// </summary>
	internal class RestRepository
	{
		// Constantes privadas
		private const string TagRoot = "Rest";
		private const string TagApi = "Api";
		private const string TagName = "Name";
		private const string TagDescription = "Description";
		private const string TagDefaultHeaders = "DefaultHeaders";
		private const string TagValue = "Value";
		private const string TagContext = "Context";
		private const string TagUrl = "Url";
		private const string TagTimeout = "Timeout";
		private const string TagType = "Type";
		private const string TagCredentials = "Credentials";
		private const string TagUser = "User";
		private const string TagPassword = "Password";
		private const string TagScope = "Scope";
		private const string TagMethod = "Method";
		private const string TagBody = "Body";
		private const string TagHeaders = "Headers";

		/// <summary>
		///		Carga los datos de una solución
		/// </summary>
		internal RestSolutionModel Load(string fileName)
		{
			RestSolutionModel solution = new RestSolutionModel();
			MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

				// Carga los datos
				if (fileML != null)
					foreach (MLNode rootML in fileML.Nodes)
						if (rootML.Name == TagRoot)
							foreach (MLNode nodeML in rootML.Nodes)
								if (nodeML.Name == TagApi)
								{
									RestModel restApi = new RestModel();

										// Asigna las propiedades
										restApi.Name = nodeML.Nodes[TagName].Value.TrimIgnoreNull();
										restApi.Description = nodeML.Nodes[TagDescription].Value.TrimIgnoreNull();
										// Añade las cabeceras predeterminadas
										LoadHeaders(nodeML, TagDefaultHeaders, restApi.DefaultHeaders);
										// Carga los contextos y métodos
										restApi.Contexts.AddRange(LoadContexts(nodeML));
										restApi.Methods.AddRange(LoadMethods(nodeML));
										// Añade la api a la colección
										solution.RestApis.Add(restApi);
								}
				// Devuelve la solución
				return solution;
		}

		/// <summary>
		///		Carga las cabeceras hijas de un nodo
		/// </summary>
		private void LoadHeaders(MLNode rootML, string tag, Dictionary<string, string> headers)
		{
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == tag)
					headers.Add(nodeML.Attributes[TagName].Value.TrimIgnoreNull(), nodeML.Attributes[TagValue].Value.TrimIgnoreNull());
		}

		/// <summary>
		///		Carga los contextos
		/// </summary>
		private List<ContextModel> LoadContexts(MLNode rootML)
		{
			List<ContextModel> contexts = new List<ContextModel>();

				// Carga los datos
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagContext)
					{
						ContextModel context = new ContextModel();

							// Añade los datos
							context.Url = nodeML.Attributes[TagUrl].Value.TrimIgnoreNull();
							context.Timeout = TimeSpan.FromMinutes(nodeML.Attributes[TagTimeout].Value.GetInt(3));
							// Añade las crdenciales
							foreach (MLNode childML in nodeML.Nodes)
								if (childML.Name == TagCredentials)
								{
									context.Credentials.Authentication = childML.Attributes[TagType].Value.GetEnum(CredentialsModel.AuthenticationType.Noauthentication);
									context.Credentials.UrlAuthority = childML.Attributes[TagUrl].Value.TrimIgnoreNull();
									context.Credentials.User = childML.Attributes[TagUser].Value.TrimIgnoreNull();
									context.Credentials.Password = childML.Attributes[TagPassword].Value.TrimIgnoreNull();
									context.Credentials.Scope = childML.Attributes[TagScope].Value.TrimIgnoreNull();
								}
							// Añade el contexto a la colección
							contexts.Add(context);
					}
				// Devuelve la lista
				return contexts;
		}

		/// <summary>
		///		Carga los métodos
		/// </summary>
		private List<MethodModel> LoadMethods(MLNode rootML)
		{
			List<MethodModel> methods = new();

				// Carga los métodos
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagMethod)
					{
						MethodModel method = new();

							// Asigna las propiedades
							method.Method = nodeML.Attributes[TagType].Value.GetEnum(MethodModel.MethodType.Get);
							method.Url = nodeML.Attributes[TagUrl].Value.TrimIgnoreNull();
							method.Body = nodeML.Nodes[TagBody].Value.TrimIgnoreNull();
							// Asigna las cabeceras
							LoadHeaders(nodeML, TagHeaders, method.Headers);
							// Añade el método a la colección
							methods.Add(method);
					}
				// Devuelve la lista de métodos
				return methods;
		}

		/// <summary>
		///		Graba los datos de una solución
		/// </summary>
		internal void Save(string fileName, RestSolutionModel solution)
		{
			MLFile fileML = new();
			MLNode rootML = fileML.Nodes.Add(TagRoot);

				// Asigna los nodos
				foreach (RestModel restApi in solution.RestApis)
				{
					MLNode nodeML = rootML.Nodes.Add(TagApi);

						// Asigna las propiedades
						nodeML.Nodes.Add(TagName, restApi.Name);
						nodeML.Nodes.Add(TagDescription, restApi.Description);
						// Añade las cabeceras
						nodeML.Nodes.AddRange(GetHeaderNodes(TagDefaultHeaders, restApi.DefaultHeaders));
						// Añade los contextos y métodos
						nodeML.Nodes.AddRange(GetContextNodes(restApi.Contexts));
						nodeML.Nodes.AddRange(GetMethodNodes(restApi.Methods));
				}
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
		}

		/// <summary>
		///		Añade las cabecenas de los nodos
		/// </summary>
		private MLNodesCollection GetHeaderNodes(string tag, Dictionary<string, string> headers)
		{
			MLNodesCollection nodesML = new();

				// Añade los nodos de cabecera
				foreach (KeyValuePair<string, string> header in headers)
				{
					MLNode nodeML = nodesML.Add(tag);

						// Añade los atributos
						nodeML.Attributes.Add(TagName, header.Key);
						nodeML.Attributes.Add(TagValue, header.Value);
				}
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene los nodos de contexto
		/// </summary>
		private MLNodesCollection GetContextNodes(List<ContextModel> contexts)
		{
			MLNodesCollection nodesML = new();

				// Añade los contextos
				foreach (ContextModel context in contexts)
				{
					MLNode rootML = nodesML.Add(TagContext);
					MLNode credentialsML;

						// Añade los atributos
						rootML.Attributes.Add(TagUrl, context.Url);
						rootML.Attributes.Add(TagTimeout, context.Timeout.TotalMinutes);
						// Añade las credenciales
						credentialsML = rootML.Nodes.Add(TagCredentials);
						credentialsML.Attributes.Add(TagType, context.Credentials.Authentication.ToString());
						credentialsML.Attributes.Add(TagUrl, context.Credentials.UrlAuthority);
						credentialsML.Attributes.Add(TagUser, context.Credentials.User);
						credentialsML.Attributes.Add(TagPassword, context.Credentials.Password);
						credentialsML.Attributes.Add(TagScope, context.Credentials.Scope);
				}
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene los nodos de los métodos
		/// </summary>
		private MLNodesCollection GetMethodNodes(List<MethodModel> methods)
		{
			MLNodesCollection nodesML = new();

				// Asigna los métodos
				foreach (MethodModel method in methods)
				{
					MLNode nodeML = nodesML.Add(TagMethod);

						// Asigna los atributos
						nodeML.Attributes.Add(TagType, method.Method.ToString());
						nodeML.Attributes.Add(TagUrl, method.Url);
						nodeML.Nodes.Add(TagBody, method.Body);
						// Añade las cabeceras
						nodeML.Nodes.AddRange(GetHeaderNodes(TagHeaders, method.Headers));
				}
				// Devuelve la colección de nodos
				return nodesML;
		}
	}
}
