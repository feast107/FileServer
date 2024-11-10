export type TerminalPayload = {
	id : string,
	message : string,
	type? : 'out' | 'error'
}


export class TerminalSession {
	private static socketTask : Promise<WebSocket> | null = null;
	private static handlers : Record<string, (line : TerminalPayload) => void> = {};

	constructor({id, path} : { id : string, path : string }) {
		this.id = id;
		this.path = path
		this.outputs = []
		this.socket = null
		this.ready = false;
		this.handlers = []
		this.input = null
		TerminalSession.handlers[id] = (payload) => {
			this.handlers.forEach(x => x(payload))
		}
		if (TerminalSession.socketTask == null) {
			TerminalSession.socketTask = new Promise((res, rej) => {
				const socket = new WebSocket('/terminal')
				socket.onopen = () => res(socket)
				socket.onerror = rej
				socket.onmessage = (e) => {
					const data = JSON.parse(e.data) as TerminalPayload
					TerminalSession.handlers[data.id](data)
				}
			})
		}
		this.socketInit = new Promise((res, rej) => {
			if (TerminalSession.socketTask == null) {
				rej()
				return;
			}
			TerminalSession.socketTask.then((s) => {
				this.socket = s;
				res()
			}).catch(rej);
		})
	}

	public id : string;
	public path : string;
	public outputs : string[];
	public ready : boolean;
	public payloads : TerminalPayload[] = [];

	public input : string | null;

	private socket : WebSocket | null;
	private readonly socketInit : Promise<void>;
	private readonly handlers : ((line : TerminalPayload) => void)[];

	public get open() {
		return this.socketInit
	}

	public onMessage(handler : (line : TerminalPayload) => void) {
		this.handlers.push(handler)
	}

	public send(line? : string | undefined) {
		if (this.socket) {
			this.socket.send(JSON.stringify({id: this.id, message: line ?? this.input}))
		}
	}

	public close() {
		this.socket?.send(JSON.stringify({id: this.id, type: 'close'}))
	}
}
