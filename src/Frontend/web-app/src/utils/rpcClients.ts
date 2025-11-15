   import { ethers } from "ethers";

   export const ethProvider = new ethers.JsonRpcProvider(import.meta.env.VITE_RPC_ETHEREUM);
   export const polygonProvider = new ethers.JsonRpcProvider(import.meta.env.VITE_RPC_POLYGON);