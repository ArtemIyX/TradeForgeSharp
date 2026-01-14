export const required = v => !!v || 'Required';
export const positive = v => Number(v) > 0 || 'Must be positive';
export const gtField = (field, label) => v =>
  Number(v) >= Number(field) || `Must be ≥ ${label}`;
