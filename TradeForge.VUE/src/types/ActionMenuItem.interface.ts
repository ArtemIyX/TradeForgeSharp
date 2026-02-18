

export interface ActionButton {
  type: 'button';
  key: string;
  label: string;
  icon?: string;
  action: (payload?: any) => void;
}

export interface ActionDivider {
  type: 'divider';
  key: string;
}

export type ActionMenuItem = ActionButton | ActionDivider;
