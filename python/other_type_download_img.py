from bs4 import BeautifulSoup
from selenium.webdriver.common.by import By
import time
import undetected_chromedriver as uc
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from urllib3.exceptions import NewConnectionError
from selenium.common.exceptions import WebDriverException
from selenium.webdriver.chrome.options import Options
import datetime
from dateutil.parser import parse
import pandas as pd
from selenium.common.exceptions import TimeoutException
import json
import requests
import os
import sys

class Lamborghini:

    def __init__(self, version_main=140):
        self.version_main= version_main
        # stop_date_str = pd.read_csv('./天下雜誌/天下雜誌_update.csv')['date'][0]
        # self.stopLineDate = pd.to_datetime(stop_date_str)  
        self.page = 1
        self.articles_list = []
        self.stop_scraping = False # 標記法

    # 拿來關閉阻擋的cookies,因為會使程式無法繼續
    def cookies_block(self,driver):

        try:
            cookies_button = WebDriverWait(driver,5).until(EC.presence_of_element_located((By.ID,'onetrust-accept-btn-handler')))
            cookies_button.click()
            print('✅成功移除cookies阻攔')
        except:
            pass


    def download_img(self):

        # 設定 logger
        class Logger:
            def __init__(self, filename="output.log"):
                self.terminal = sys.stdout   # 原本的 console
                self.log = open(filename, "a", encoding="utf-8")

            def write(self, message):
                self.terminal.write(message)   # 照樣印到 console
                self.log.write(message)        # 同時寫進 log
                self.log.flush()               # 立即寫入檔案

            def flush(self):
                pass  # for Python buffer 需求
        
        # 把 stdout 和 stderr 都導向 Logger
        sys.stdout = Logger("./PastModels.log")
        sys.stderr = sys.stdout

        try:

            df = pd.read_json(r"./PastModels.json")

            for idx,info in df.iterrows():
                
                try:
                    print(idx)
                    title = info['title']
                    path_count = 1
                    path = f"./PastModels_Lambo_pic/{title}"
                    while os.path.exists(path):
                        path = f"./PastModels_Lambo_pic/{title}/{path_count}"
                        path_count += 1

                    os.makedirs(path, mode=0o777)
                    
                    imgs = info["img"]
                
                except Exception as e:
                    print(f'❌建立資料夾出問題: {e}')

                count = 0
                
                for img in imgs:
                    try:
                        
                        thumbnail = requests.get(img)
                        count += 1
                        
                        file: str = os.path.join(path,str(count))
                        with open(f'{file}.png', 'wb') as f:
                            f.write(thumbnail.content)

                    except Exception as e:
                        print(f'❌下載單張圖片出問題: {e}')

                print("✅成功抓到完整圖片")
                


            
        except Exception as e:
            print(f'❌下載圖片出問題: {e}')
    

if __name__ == "__main__":
    b = Lamborghini()
    b.download_img()

   


