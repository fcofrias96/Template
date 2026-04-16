#nullable enable
//
// ✅ TODO EL CÓDIGO “CORREGIDO” / MEJORADO
// - Key builder: escape + Invariant + hash si muy largo
// - Namespace version: striped locks (no crece infinito)
// - Policy registry: cast seguro + mejor error
// - Factory: valida NamespaceTtl >= DataTtl + namespaces estables
// - Contexts: hashes para firmas grandes (permission/filter)
//

public interface ICacheKeyBuilder
{
    string Build(params object?[] parts);
}
