import { MessageModel } from './message';

export interface ChatModel {
  chatId: string;
  documentId: string;
  documentName: string;
  messages: MessageModel[];
  documentUniqueName: string;
}
