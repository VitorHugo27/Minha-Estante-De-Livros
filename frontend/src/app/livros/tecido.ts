// Escolhe uma das cinco cores de tecido a partir do título.
// O mesmo título dá sempre a mesma cor, na capa e na lombada.
export function corDoTecido(titulo: string): number {
  let soma = 0;
  for (const letra of titulo) {
    soma += letra.charCodeAt(0);
  }
  return soma % 5;
}
