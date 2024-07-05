import axios from "axios";
import {FileSystemInfo} from "../models/FileSystemInfo.ts";

const request = axios.create({})

export default {
	async tree(...route : string[]) : Promise<FileSystemInfo[]> {
		return (await request.get("/tree/" + route.join('/'))).data;
	},
	url(...route : string[]) {
		return request.getUri(
			{
				url: "/tree/" + route.join('/')
			})
	}
}
