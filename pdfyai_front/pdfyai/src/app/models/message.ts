export interface MessageModel {
  messageId: string;
  content: string;
  role: string;
  hasCodeBloc?: boolean;
}
